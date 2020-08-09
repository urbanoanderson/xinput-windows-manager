using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace XinputWindowsManager
{
    public class WindowsMouseCursor
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        public static Point GetPosition()
        {
            Point position = default;
            GetCursorPos(out position);

            return position;
        }

        public static void SetPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public static void SendLeftClick()
        {
            Point p = GetPosition();
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)p.X, (uint)p.Y, 0, (UIntPtr)0);
        }

        public static void SendRightClick()
        {
            Point p = GetPosition();
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint)p.X, (uint)p.Y, 0, (UIntPtr)0);
        }
    }
}
