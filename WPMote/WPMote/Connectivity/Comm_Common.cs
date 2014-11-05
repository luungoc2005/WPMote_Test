using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace WPMote.Connectivity
{
    class Comm_Common
        //Note: this class also handles closing of sockets
    {
        public static delegate void ConnectedEvent(StreamSocket objRetSocket);
        public delegate void MessageReceived(Comm_Message objMessage);
        
        public event MessageReceived OnMessageReceived;

        private CommMode objMode;
        StreamSocket objMainSocket;

        Comm_Bluetooth objBluetooth;
        Comm_TCP objTCP;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        public bool IsConnected() { return (objMainSocket == null); }

        public Comm_Common(CommMode mode, string strHost = "127.0.0.1", int intTCPPort = 8046)
        {
            objMode = mode;

            switch (mode)
            {
                case CommMode.Bluetooth:
                    objBluetooth = new Comm_Bluetooth();

                    objBluetooth.Connected += ConnectedHandler;
                    
                    //TODO: Bluetooth - interface handling

                    break;
                case CommMode.TCP:
                    objTCP = new Comm_TCP();
                    objTCP.Connect(strHost, intTCPPort);

                    break;
                default:
                    break;
            }
        }


        public void ConnectedHandler(StreamSocket objSocket)
        {
            objMainSocket = objSocket;
        }

        public void Close()
        {
            try
            {
                if (!IsConnected()) //Not yet connected
                {

                }
                else
                {
                    objMainSocket.Dispose();
                    objMainSocket = null;
                }
            }
            catch
            {
            }
        }

        public void SendBytes(byte[] buffer)
        {

        }
    }
}
