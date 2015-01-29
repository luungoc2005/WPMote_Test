using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WPMote_Desk.Win32
{
    abstract class Win32API
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern Int16 GetAsyncKeyState(int vKey);
        [DllImport("user32.dll")]
        public static extern int GetCursorPos(ref POINTAPI lpPoint);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_KEYUP = 0x2;
        public const int KEYEVENTF_SCANCODE = 0x8;
        public const int KEYEVENTF_EXTENDEDKEY = 0x1;

        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const int MOUSEEVENTF_LEFTDOWN = 0x2;
        public const int MOUSEEVENTF_LEFTUP = 0x4;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_MOVE = 0x1;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_WHEEL = 0x800;
        public const int MOUSEEVENTF_XDOWN = 0x80;
        public const int MOUSEEVENTF_XUP = 0x100;
        public const int VK_LBUTTON = 0x1;
        public const int VK_RBUTTON = 0x2;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            internal int x;
            internal int y;
        }


    }
}
