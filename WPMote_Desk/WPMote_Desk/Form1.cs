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

namespace WPMote_Desk
{
    public partial class Form1 : Form
    {
        Comm_Common objComm;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            objComm = new Comm_Common(Comm_Common.CommMode.TCP);
            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
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
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo(Comm_TCP.LocalIPAddress(),Environment.MachineName).ToByteArray);
        }
    }
}
