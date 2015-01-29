using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPMote_Desk.Connectivity;
using System.Diagnostics;
using WPMote_Desk.Connectivity.Messages;
using WPMote_Desk.Processor;

namespace WPMote_Desk
{
    public partial class Form1 : Form
    {
        Comm_Common objComm;
        // Form2 objFrm2;
        MouseProcessor objProc;

        public Form1()
        {
            InitializeComponent();
        }

        private void objComm_OnConnected(object sender, EventArgs e)
        {
            this.BeginInvoke((Action)(() =>
            {
                this.Text = "WPMote [Connected] - Local IP: " + Comm_TCP.LocalIPAddress();
            }));
        }

        void Events_OnClickReceived(bool RClick, bool LClick)
        {
            Win32.MousePointer.RightButtonDown = RClick;
            Win32.MousePointer.LeftButtonDown = LClick;
        }

        void Events_OnCompressedAccelDataReceived(Int16 X, Int16 Y, Int16 Z)
        {
            Debug.Print(((float)X / 10000).ToString() + "," + ((float)Y / 10000).ToString() + "," + ((float)Z / 10000).ToString());

            objProc.AddReading(new Simple3DVector((float)X / 10000, (float)Y / 10000, (float)Z / 10000));            
        }

        List<Point> lstPosQueue = new List<Point>();

        void OnClientInfoReceived(string IPAddress, string DeviceName)
        {
            Debug.Print("ClientInfo received: " + IPAddress + " (" + DeviceName + ")");
            objComm.classTCPHost = IPAddress;
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo(Comm_TCP.LocalIPAddress(), Environment.MachineName).ToByteArray, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (objComm!=null)
            {
                objComm.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "WPMote - Local IP: " + Comm_TCP.LocalIPAddress();
            Debug.Print(System.Net.IPAddress.Broadcast.ToString());

            objComm = new Comm_Common(Comm_Common.CommMode.TCP);

            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
            objComm.Events.OnAccelerometerDataReceived += Events_OnAccelerometerDataReceived;
            objComm.Events.OnCompressedAccelDataReceived += Events_OnCompressedAccelDataReceived;
            objComm.Events.OnClickReceived += Events_OnClickReceived;
            objComm.Events.OnKeyBDReceived += Events_OnKeyBDReceived;
            objComm.OnConnected += objComm_OnConnected;

            //objFrm2 = new Form2();
            //objFrm2.Show();

            objProc = MouseProcessor.Instance;
            objProc.Start();

            objProc.OnDeviceTilt += objProc_OnDeviceTilt;

            objComm.Connect();
        }

        void objProc_OnDeviceTilt(MouseProcessor.TiltDirections direction, bool value)
        {
            if (checkBox1.Checked)
            {
                switch (direction)
                {
                    case MouseProcessor.TiltDirections.Forward:
                        Win32.Win32API.keybd_event(0, 0xC8, Win32.Win32API.KEYEVENTF_SCANCODE | (value ? 0 : Win32.Win32API.KEYEVENTF_KEYUP), 0);
                        break;
                    case MouseProcessor.TiltDirections.Backward:
                        Win32.Win32API.keybd_event(0, 0xD0, Win32.Win32API.KEYEVENTF_SCANCODE | (value ? 0 : Win32.Win32API.KEYEVENTF_KEYUP), 0);
                        break;
                    case MouseProcessor.TiltDirections.Left:
                        Win32.Win32API.keybd_event(0, 0xCB, Win32.Win32API.KEYEVENTF_SCANCODE | (value ? 0 : Win32.Win32API.KEYEVENTF_KEYUP), 0);
                        break;
                    case MouseProcessor.TiltDirections.Right:
                        Win32.Win32API.keybd_event(0, 0xCD, Win32.Win32API.KEYEVENTF_SCANCODE | (value ? 0 : Win32.Win32API.KEYEVENTF_KEYUP), 0);
                        break;
                    default:
                        break;
                }                
            }
        }

        void Events_OnKeyBDReceived(byte KeyBD, bool KeyState)
        {
            Win32.Win32API.keybd_event(0, KeyBD, Win32.Win32API.KEYEVENTF_SCANCODE | (KeyState ? 0 : Win32.Win32API.KEYEVENTF_KEYUP), 0);
        }

        private void Events_OnAccelerometerDataReceived(float X, float Y, float Z, int flags)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Win32.MousePointer.Position.ToString() + "\r\n" + Win32.MousePointer.LeftButtonDown.ToString();
        }

        private void tmrSmooth_Tick(object sender, EventArgs e)
        {
            if (objProc == null) return;
            label2.Text = objProc.readingsQueue.Count.ToString() + "\r\n" +
                "X: " + objProc.currentVelocity.X.ToString() + "\r\n" +
                "Y: " + objProc.currentVelocity.Y.ToString() + "\r\n" +
                "Z: " + objProc.currentVelocity.Z.ToString() + "\r\n" +
                "lag: " + objProc.lngAvgPing.ToString(); ;
        }

    }
}
