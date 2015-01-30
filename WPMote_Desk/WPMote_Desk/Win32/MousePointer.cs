using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WPMote_Desk.Win32
{
    class MousePointer
    {

        //Get & Set absolute coordinates
        public static Point Position
        {
            get
            {
                var _ret=new Win32API.POINTAPI();

                Win32API.GetCursorPos(ref _ret);

                return new Point(_ret.x, _ret.y);
            }
            set
            {
                //Win32API.mouse_event((Win32API.MOUSEEVENTF_ABSOLUTE | Win32API.MOUSEEVENTF_MOVE),
                //    value.X, value.Y, 0, 0);
                Cursor.Position = value;
            }
        }

        public static bool RightButtonDown
        {
            get
            {
                if (SystemInformation.MouseButtonsSwapped)
                {
                    return !Win32API.GetAsyncKeyState(Win32API.VK_LBUTTON).Equals(0);
                }
                else
                {
                    return !Win32API.GetAsyncKeyState(Win32API.VK_RBUTTON).Equals(0);
                }
            }
            set
            {
                if (RightButtonDown != value)
                {
                    if (value)
                    {
                        Win32API.mouse_event((SystemInformation.MouseButtonsSwapped) ?
                            Win32API.MOUSEEVENTF_LEFTDOWN : Win32API.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    }
                    else
                    {
                        Win32API.mouse_event((SystemInformation.MouseButtonsSwapped) ?
                            Win32API.MOUSEEVENTF_LEFTUP : Win32API.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                    }
                }
            }
        }

        public static bool LeftButtonDown
        {
            get
            {
                if (SystemInformation.MouseButtonsSwapped)
                {
                    return !Win32API.GetAsyncKeyState(Win32API.VK_RBUTTON).Equals(0);
                }
                else
                {
                    return !Win32API.GetAsyncKeyState(Win32API.VK_LBUTTON).Equals(0);
                }
            }
            set
            {
                if (LeftButtonDown!=value)
                {
                    if (value)
                    {
                        Win32API.mouse_event((SystemInformation.MouseButtonsSwapped) ?
                            Win32API.MOUSEEVENTF_RIGHTDOWN : Win32API.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    }
                    else
                    {
                        Win32API.mouse_event((SystemInformation.MouseButtonsSwapped) ?
                            Win32API.MOUSEEVENTF_RIGHTUP : Win32API.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    }
                }
            }
        }

        //Set relative coordinates
        public static void APIMove(Point MoveBy)
        {
            Win32API.mouse_event(Win32API.MOUSEEVENTF_MOVE,
                MoveBy.X, MoveBy.Y, 0, 0);
        }

        public static void Move(Point MoveBy)
        {
            Point pos=Win32.MousePointer.Position;
            Win32.MousePointer.Position = new Point(pos.X + MoveBy.X,
                                                    pos.Y + MoveBy.Y);
        }
    }
}
