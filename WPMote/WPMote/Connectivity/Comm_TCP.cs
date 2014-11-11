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
        #region "Common variables"

        int intPort = 8019;
        StreamSocket objClient;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

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
        
        public async void Connect(string strHost, int intPort)
        {
            try
            {
                objClient = new StreamSocket();
                await objClient.ConnectAsync(new HostName(strHost), intPort.ToString());
                if (Connected != null) Connected(objClient);
            }
            catch (Exception ex)
            {
                if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                if (objClient != null) objClient.Dispose();
            }
        }


        #endregion

        #region "Private methods"

        #endregion
        
    }
}
