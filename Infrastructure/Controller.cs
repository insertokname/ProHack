using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Application;

namespace Infrastructure
{
    public class Controller
    {
        [DllImport("user32.dll")]
        private static extern nint GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern nint GetMessageExtraInfo();

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public nint dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public nint dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }


        /// <summary>
        /// Sends a key down event to the specified process using SendInput
        /// This works better with Unity games and works in background
        /// </summary>
        /// <param name="process">The target process</param>
        /// <param name="virtualKeyCode">The virtual key code to send (e.g., 0x57 for 'W', 0x41 for 'A')</param>
        /// <returns>True if the input was sent successfully</returns>
        public static bool SendKeyDown(Process process, ushort virtualKeyCode)
        {
            if (process == null || process.HasExited)
            {
                return false;
            }

            // Bring window to foreground first
            nint hWnd = process.MainWindowHandle;
            if (hWnd != nint.Zero)
            {
                SetForegroundWindow(hWnd);
            }

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = Constants.Keyboard.INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = virtualKeyCode;
            inputs[0].u.ki.wScan = (ushort)MapVirtualKey(virtualKeyCode, Constants.Keyboard.MAPVK_VK_TO_VSC);
            inputs[0].u.ki.dwFlags = 0; // 0 for key down
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = GetMessageExtraInfo();

            uint result = SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
            return result > 0;
        }

        /// <summary>
        /// Sends a key up event to the specified process using SendInput
        /// This works better with Unity games and works in background
        /// </summary>
        /// <param name="process">The target process</param>
        /// <param name="virtualKeyCode">The virtual key code to send (e.g., 0x57 for 'W', 0x41 for 'A')</param>
        /// <returns>True if the input was sent successfully</returns>
        public static bool SendKeyUp(Process process, ushort virtualKeyCode)
        {
            if (process == null || process.HasExited)
            {
                return false;
            }

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = Constants.Keyboard.INPUT_KEYBOARD;
            inputs[0].u.ki.wVk = virtualKeyCode;
            inputs[0].u.ki.wScan = (ushort)MapVirtualKey(virtualKeyCode, Constants.Keyboard.MAPVK_VK_TO_VSC);
            inputs[0].u.ki.dwFlags = Constants.Keyboard.KEYEVENTF_KEYUP;
            inputs[0].u.ki.time = 0;
            inputs[0].u.ki.dwExtraInfo = GetMessageExtraInfo();

            uint result = SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
            return result > 0;
        }

        /// <summary>
        /// Sends a complete key press (down + up) to the specified process
        /// </summary>
        /// <param name="process">The target process</param>
        /// <param name="virtualKeyCode">The virtual key code to send (e.g., 0x57 for 'W', 0x41 for 'A')</param>
        /// <param name="delayMs">Optional delay between key down and key up in milliseconds</param>
        /// <returns>True if both inputs were sent successfully</returns>
        public static bool SendKeyPress(Process process, ushort virtualKeyCode, int delayMs = 50)
        {
            bool keyDownSuccess = SendKeyDown(process, virtualKeyCode);
            if (!keyDownSuccess)
            {
                return false;
            }

            if (delayMs > 0)
            {
                Thread.Sleep(delayMs);
            }

            bool keyUpSuccess = SendKeyUp(process, virtualKeyCode);
            return keyUpSuccess;
        }
    }
}
