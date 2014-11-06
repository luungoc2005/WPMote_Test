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
    }
}
