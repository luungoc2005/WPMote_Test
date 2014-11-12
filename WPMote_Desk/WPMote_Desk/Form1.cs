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
        Form2 objFrm2;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            objComm = new Comm_Common(Comm_Common.CommMode.TCP);
            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
            objComm.Events.OnAccelerometerDataReceived += Events_OnAccelerometerDataReceived;

            objFrm2 = new Form2();
            objFrm2.Show();
        }

        void Events_OnAccelerometerDataReceived(float X, float Y, float Z, int flags)
        {
            this.BeginInvoke((Action)(() =>
            {
                Point pos = MouseProcessor.AccelToCoord(X, Y);
                objFrm2.Left = pos.X;
                objFrm2.Top = pos.Y;
            }));


        }

        private void OnClientInfoReceived(string IPAddress, string DeviceName)
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

    }
}
