using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.ControlsEx;
using System.Windows;
using WPMote.Resources;
using WPMote.Connectivity;
using WPMote.Connectivity.Messages;
using Microsoft.Phone.Applications.Common;

namespace WPMote
{
    public class xamlBinder
    {
        public Comm_Common objComm;
        public Comm_Common Communicator
        {
            set {objComm = value;}
        }

        public xamlBinder()
        {

        }
        

        public void assignHandler(string name, string type, string handler, FrameworkElement content)
        {
            switch (type)
            {
                case "ButtonEx":
                {
                    addInputBtn((ButtonEx)content.FindName(name), handler);
                    break;
                }
                case "Button":
                {
                    break;
                }
                case "CheckBox":
                {
                    break;
                }
                case "RightMouse":
                {
                    break;
                }
                case "Message":
                {
                    break;
                }
            }
        }

        private void addInputBtn(ButtonEx targetButton, string handler)
        {
            switch (handler)
            {
                case "ScanCode":
                    {
                        targetButton.TouchDown += new ButtonEx.TouchEventHandler(InputBtn_MouseLeftButtonDown);
                        targetButton.TouchDragOutside += new ButtonEx.TouchEventHandler(InputBtn_MouseLeftButtonUp);
                        targetButton.TouchUpInside += new ButtonEx.TouchEventHandler(InputBtn_MouseLeftButtonUp);

                        int value = Convert.ToInt32((string)targetButton.Tag, 16);
                        Byte[] bytes = BitConverter.GetBytes(value);
                        targetButton.Tag = new inputData(bytes[0], false);
                        break;
                    }
                case "LeftMouseClick":
                    {
                        targetButton.TouchDown += new ButtonEx.TouchEventHandler(lBtn_MouseLeftButtonDown);
                        targetButton.TouchDragOutside += new ButtonEx.TouchEventHandler(lBtn_MouseLeftButtonUp);
                        targetButton.TouchUpInside += new ButtonEx.TouchEventHandler(lBtn_MouseLeftButtonUp);
                        break;
                    }
                case "RightMouseClick":
                    {
                        targetButton.TouchDown += new ButtonEx.TouchEventHandler(rBtn_MouseLeftButtonDown);
                        targetButton.TouchDragOutside += new ButtonEx.TouchEventHandler(rBtn_MouseLeftButtonUp);
                        targetButton.TouchUpInside += new ButtonEx.TouchEventHandler(rBtn_MouseLeftButtonUp);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private struct inputData
        {
            public byte scanCode;
            public bool extended;
            public inputData(byte c, bool e = false)
            {
                scanCode = c;
                extended = e;
            }
        }

        public void InputBtn_MouseLeftButtonDown(object sender, Point e)
        {
            try
            {
                ButtonEx btn = (ButtonEx)sender;
                //string temp = (string)btn.Content;
                //btn.Content = btn.Tag;
                //btn.Tag = temp;
                inputData data = (inputData)((ButtonEx)sender).Tag;
                objComm.SendBytes(new MsgCommon.KeyBDReceived(data.scanCode, ((ButtonEx)sender).IsPressed, false).ToByteArray);
            }
            catch
            {
            }
        }

        public void InputBtn_MouseLeftButtonUp(object sender, Point e)
        {
            try
            {
                ButtonEx btn = (ButtonEx)sender;
                //string temp = (string)btn.Content;
                //btn.Content = btn.Tag;
                //btn.Tag = temp;
                inputData data = (inputData)((ButtonEx)sender).Tag;
                objComm.SendBytes(new MsgCommon.KeyBDReceived(data.scanCode, ((ButtonEx)sender).IsPressed, false).ToByteArray);
            }
            catch
            {
            }
        }

        public void lBtn_MouseLeftButtonDown(object sender, Point e)
        {
            ButtonEx btn = (ButtonEx)sender;
            objComm.SendBytes(new MsgCommon.ClickReceived(btn.IsPressed, btn.IsPressed).ToByteArray);
        }

        public void lBtn_MouseLeftButtonUp(object sender, Point e)
        {
            ButtonEx btn = (ButtonEx)sender;
            objComm.SendBytes(new MsgCommon.ClickReceived(btn.IsPressed, btn.IsPressed).ToByteArray);
        }

        public void rBtn_MouseLeftButtonDown(object sender, Point e)
        {
            ButtonEx btn = (ButtonEx)sender;
            objComm.SendBytes(new MsgCommon.ClickReceived(btn.IsPressed, btn.IsPressed).ToByteArray);
        }

        public void rBtn_MouseLeftButtonUp(object sender, Point e)
        {
            ButtonEx btn = (ButtonEx)sender;
            objComm.SendBytes(new MsgCommon.ClickReceived(btn.IsPressed, btn.IsPressed).ToByteArray);
        }

    }
}