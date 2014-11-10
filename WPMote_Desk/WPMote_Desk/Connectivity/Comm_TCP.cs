using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using WPMote.Connectivity.Messages;

namespace WPMote_Desk.Connectivity
{
    public class Comm_TCP
    {
        TcpListener objServer;
        TcpClient objClient;
        int intPort=8046;
        bool bListening = false;
        private Thread tskListen;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

        private Action<NetworkStream> OnConnected;

        NetworkStream objStream;

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

        public void StartListen()
        {
            objServer = new TcpListener(IPAddress.Parse("127.0.0.1"), intPort);
            
            tskListen = new Thread(() => ListenThread());

            OnConnected = new Action<NetworkStream>((NetworkStream s) => OnConnectedEvent(s));

            tskListen.Start();
        }

        protected void OnConnectedEvent(NetworkStream s)
        {
            StopListen(); //TODO: Verify this line

            //SETTINGS
            objClient.ReceiveBufferSize = Comm_Message.BUFFER_SIZE;
            objClient.SendBufferSize = Comm_Message.BUFFER_SIZE;

            if (Connected!=null) Connected(s);
        }

        public void ListenThread()
        {
            objServer.Start();

            while (true)
            {
                objClient = objServer.AcceptTcpClient();

                objStream = objClient.GetStream();

                OnConnected.Invoke(objStream);
            }

        }

        public void StopListen()
        {
            tskListen.Abort();
            objServer.Stop();
        }


    }
}
