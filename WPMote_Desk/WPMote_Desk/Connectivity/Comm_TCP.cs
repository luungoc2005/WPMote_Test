using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using WPMote_Desk.Connectivity.Messages;

namespace WPMote_Desk.Connectivity
{
    public class Comm_TCP
    {
        #region "Common variables"

        TcpListener objServer;
        TcpClient objClient;
        int intPort = 8046;
        private Thread tskListen;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

        private Action<NetworkStream> OnConnected;

        NetworkStream objStream;

        #endregion

        #region "Shared methods"
        //http://stackoverflow.com/questions/6803073/get-local-ip-address-c-sharp
        public static string LocalIPAddress()
        {
            string localIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (IsLocalIpAddress(ip.ToString()))
                {
                    localIP += "\r\n" + ip.ToString();
                    //break;
                }
            }
            return localIP;
        }

        public static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        #endregion

        #region "Class properties"

        public int Port
        {
            get
            {
                return intPort;
            }
            set
            {
                if (value > 0) intPort = value;
            }
        }

        #endregion

        #region "Public methods"

        public void StartListen()
        {
            objServer = new TcpListener(IPAddress.Any, intPort);

            tskListen = new Thread(() => ListenThread());

            OnConnected = new Action<NetworkStream>((NetworkStream s) => OnConnectedEvent(s));

            tskListen.Start();
        }

        public void StopListen()
        {
            tskListen.Abort();
            objServer.Stop();
        }

        #endregion

        #region "Private methods"

        protected void OnConnectedEvent(NetworkStream s)
        {
            //StopListen(); //TODO: Verify this line

            //SETTINGS
            objClient.ReceiveBufferSize = MsgCommon.BUFFER_SIZE;
            objClient.SendBufferSize = MsgCommon.BUFFER_SIZE;

            if (Connected != null) Connected(s);
        }

        private void ListenThread()
        {
            objServer.Start();

            while (true)
            {
                objClient = objServer.AcceptTcpClient();

                objClient.NoDelay = true;

                objStream = objClient.GetStream();

                OnConnected.Invoke(objStream);
            }
        }

        #endregion

    }
}
