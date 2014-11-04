using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace WPMote_Desk.Connectivity
{
    public class Comm_TCP
    {
        TcpListener objServer;
        int intPort=8046;

        public int Port
        {
            get
            {
                return intPort;
            }
            set
            {
                if (value > 0) { intPort = value; }
            }
        }

        public void StartListen()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            objServer = new TcpListener(localAddr, intPort);
            objServer.Start();
        }

        public void StopListen()
        {
            if (objServer == null) { return; }

            try
            {
                objServer.Stop();
            }
            catch
            {
                
                throw;
            }
        }
    }
}
