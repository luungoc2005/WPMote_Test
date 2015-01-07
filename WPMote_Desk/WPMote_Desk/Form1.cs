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

        double currentVelocityX = 0;
        double currentVelocityY = 0;

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

            objProc = new MouseProcessor();
        }

        void Events_OnClickReceived(bool RClick, bool LClick)
        {
            Win32.MousePointer.RightButtonDown = RClick;
            Win32.MousePointer.LeftButtonDown = LClick;
        }


        long lngPrevious;
        long lngPing;
        long lngAvgPing;
        
        const int intSamples = 30;
        long[] arrSamples = new long[intSamples - 1];
        int intCount = 0;
        long lngSum = 0;

        bool _init = false;

        void Events_OnCompressedAccelDataReceived(Int16 X, Int16 Y, Int16 Z)
        {
            //Win32.MousePointer.Move(new Point(pos.X-lastpos.X,pos.Y-lastpos.Y));

            Point pos = MouseProcessor.AccelToCoord((float)X / 10000, (float)Y / 10000, (float)Z/10000);
            Debug.Print(((float)X / 10000).ToString() + "," + ((float)Y / 10000).ToString() + "," + ((float)Z / 10000).ToString());

            lstPosQueue.Add(pos);

            lastpos = pos;

            this.BeginInvoke((Action)(() =>
            {
                //var rPos = new Point(pos.X - lastpos.X, pos.Y - lastpos.Y);
                //objFrm2.Left = pos.X;
                //objFrm2.Top = pos.Y;

                tmrSmooth.Enabled = true;
            }));

            if (lngPrevious == 0)
            {
                lngPrevious = DateTime.Now.Ticks;
            }
            else
            {
                //Win32.MousePointer.Move(new Point(pos.X - lastpos.X, pos.Y - lastpos.Y));
                currentVelocityX += X / 10000;
                currentVelocityY += Y / 10000;
                lastpos = pos;
                lngPing = (DateTime.Now.Ticks - lngPrevious) / TimeSpan.TicksPerMillisecond;

                lngPrevious = DateTime.Now.Ticks;

                lngSum += lngPing;
                lngSum -= arrSamples[intCount];
                arrSamples[intCount] = lngPing;

                intCount += 1;
                if (intCount >= arrSamples.Length)
                {
                    if (_init == false) _init = true;
                    intCount = 0;
                }

                lngAvgPing = (_init) ? (lngSum / intSamples) : (lngSum / intCount);

                //Debug.Print("Ping {0} ms", lngAvgPing);
            }
        }

        Point lastpos;
        List<Point> lstPosQueue = new List<Point>();

        void Events_OnAccelerometerDataReceived(float X, float Y, float Z, int flags)
        {
            Point pos = MouseProcessor.AccelToCoord(X, Y, Z);
            
            lastpos = pos;

            this.BeginInvoke((Action)(() =>
            {
                //var rPos = new Point(pos.X - lastpos.X, pos.Y - lastpos.Y);
                //objFrm2.Left = pos.X;
                //objFrm2.Top = pos.Y;

                tmrSmooth.Enabled = true;
            }));

            if (lngPrevious == 0)
            {
                lngPrevious = DateTime.Now.Ticks;
            }
            else
            {
                Win32.MousePointer.Move(new Point(pos.X - lastpos.X, pos.Y - lastpos.Y));
                lastpos = pos;
                lngPing = (DateTime.Now.Ticks - lngPrevious) / TimeSpan.TicksPerMillisecond;

                lngPrevious = DateTime.Now.Ticks;

                lngSum += lngPing;
                lngSum -= arrSamples[intCount];
                arrSamples[intCount] = lngPing;

                intCount += 1;
                if (intCount >= arrSamples.Length)
                {
                    if (_init == false) _init = true;
                    intCount = 0;
                }

                lngAvgPing = (_init) ? (lngSum / intSamples) : (lngSum / intCount);

                Debug.Print("Ping {0} ms", lngAvgPing);
            }
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
            if (lstPosQueue.Count>0)
            {
                //Point targetpos = lstPosQueue[0];
                //Point movevector = new Point(targetpos.X - Win32.MousePointer.Position.X, targetpos.Y - Win32.MousePointer.Position.Y);

                //lstPosQueue.RemoveAt(0);

                //int intSmoothFactor = MsgInterval / tmrSmooth.Interval;
                //if (intSmoothFactor > 0)
                //{
                //    Win32.MousePointer.Move(new Point(movevector.X / intSmoothFactor, movevector.Y / intSmoothFactor));
                //}
                int multiplyFactor=9;
                Win32.MousePointer.Move(new Point((int)(currentVelocityX * multiplyFactor), (int)(currentVelocityY * multiplyFactor)));
            }
        }
    }
}
