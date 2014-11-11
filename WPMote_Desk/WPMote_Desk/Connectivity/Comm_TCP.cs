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
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
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
            objServer = new TcpListener(IPAddress.Parse("127.0.0.1"), intPort);

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

                objStream = objClient.GetStream();

                OnConnected.Invoke(objStream);
            }
        }

        #endregion

    }
}
