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

        private void button1_Click(object sender, EventArgs e)
        {
            objComm = new Comm_Common(Comm_Common.CommMode.TCP);
            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
            objComm.Events.OnAccelerometerDataReceived += Events_OnAccelerometerDataReceived;
            objComm.Events.OnCompressedAccelDataReceived += Events_OnCompressedAccelDataReceived;
            objComm.Events.OnClickReceived += Events_OnClickReceived;

            //objFrm2 = new Form2();
            //objFrm2.Show();

            objProc = MouseProcessor.Instance;
            objProc.Start();
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

        void Events_OnAccelerometerDataReceived(float X, float Y, float Z, int flags)
        {

        }

        void OnClientInfoReceived(string IPAddress, string DeviceName)
        {
            Debug.Print("ClientInfo received: " + IPAddress + " (" + DeviceName + ")");
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
            Debug.Print(Comm_TCP.LocalIPAddress());           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo(Comm_TCP.LocalIPAddress(),Environment.MachineName).ToByteArray);
            Debug.Print("Send clicked");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = Win32.MousePointer.Position.ToString() + "\r\n" + Win32.MousePointer.LeftButtonDown.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Win32.MousePointer.Position = new Point(0, 0);
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            Win32.MousePointer.LeftButtonDown = true;
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

        private void button6_Click(object sender, EventArgs e)
        {
            Win32.MousePointer.Move(new Point(200, 200));
        }
    }
}
