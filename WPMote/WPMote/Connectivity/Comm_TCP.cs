using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;

namespace WPMote.Connectivity
{
    class Comm_TCP
    {
        int intPort = 8019;
        StreamSocket objClient;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

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

        public async void Connect(string strHost, int intPort)
        {
            try
            {
                objClient = new StreamSocket();
                await objClient.ConnectAsync(new HostName(strHost), intPort.ToString());
                Connected(objClient);
            }
            catch (Exception ex)
            {
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }
                
                objClient.Dispose();
            }
        }

    }
}
