using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Application
{
    public class VideoManager
    {
        private const int PW_CLIENTONLY = 0x00000001;
        private const int SRCCOPY = 0x00CC0020;
        private const int CAPTUREBLT = 0x40000000;

        public Bitmap? CaptureProcessWindow(MemoryManager memoryManager, bool clientAreaOnly = false)
        {
            if (memoryManager == null)
            {
                throw new ArgumentNullException(nameof(memoryManager));
            }

            Process? process = memoryManager.Process;
            if (process == null || process.HasExited)
            {
                return null;
            }

            IntPtr windowHandle = process.MainWindowHandle;
            if (windowHandle == IntPtr.Zero)
            {
                return null;
            }

            if (!GetWindowRect(windowHandle, out RECT rect))
            {
                return null;
            }

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            if (width <= 0 || height <= 0)
            {
                return null;
            }

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            bool success = TryPrintWindow(windowHandle, bitmap, clientAreaOnly);
            if (!success)
            {
                success = TryBlitWindow(windowHandle, bitmap);
            }

            if (!success)
            {
                bitmap.Dispose();
                return null;
            }

            return bitmap;
        }

        private static bool TryPrintWindow(IntPtr windowHandle, Bitmap bitmap, bool clientAreaOnly)
        {
            using Graphics graphics = Graphics.FromImage(bitmap);
            IntPtr hdc = graphics.GetHdc();
            try
            {
                return PrintWindow(windowHandle, hdc, clientAreaOnly ? PW_CLIENTONLY : 0);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        private static bool TryBlitWindow(IntPtr windowHandle, Bitmap bitmap)
        {
            using Graphics windowGraphics = Graphics.FromHwnd(windowHandle);
            using Graphics bitmapGraphics = Graphics.FromImage(bitmap);

            IntPtr hdcSrc = windowGraphics.GetHdc();
            IntPtr hdcDest = bitmapGraphics.GetHdc();

            try
            {
                bool bltSucceeded = BitBlt(
                    hdcDest,
                    0,
                    0,
                    bitmap.Width,
                    bitmap.Height,
                    hdcSrc,
                    0,
                    0,
                    SRCCOPY | CAPTUREBLT);

                return bltSucceeded;
            }
            finally
            {
                bitmapGraphics.ReleaseHdc(hdcDest);
                windowGraphics.ReleaseHdc(hdcSrc);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
            IntPtr hdcDest,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            int dwRop);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}