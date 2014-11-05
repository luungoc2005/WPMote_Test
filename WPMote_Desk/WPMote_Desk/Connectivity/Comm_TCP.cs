using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

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
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            objServer = new TcpListener(localAddr, intPort);

            objServer.Start();

            tskListen = new Thread(() => ListenThread());

            OnConnected = new Action<NetworkStream>((NetworkStream s) => Connected(s));

            tskListen.Start();
        }

        public void ListenThread()
        {
            objClient = objServer.AcceptTcpClient();
            objServer.Stop();

            objStream = objClient.GetStream();

            OnConnected.Invoke(objStream);
        }

        public void StopListen()
        {
            tskListen.Abort();
            objServer.Stop();
        }


    }
}
