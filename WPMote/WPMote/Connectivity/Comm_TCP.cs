using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace WPMote.Connectivity
{
    class Comm_TCP
    {
        #region "Common variables"

        int intPort = 8019;
        string strHostName = "";
        StreamSocket objClient;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

        #endregion

        #region "Shared methods"
        //http://developer.nokia.com/community/wiki/How_to_get_the_device_IP_addresses_on_Windows_Phone
        public static string LocalIPAddress()
        {
            List<string> ipAddresses = new List<string>();
            var hostnames = NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                //IanaInterfaceType == 71 => Wifi
                //IanaInterfaceType == 6 => Ethernet (Emulator)
                if (hn.IPInformation != null &&
                    (hn.IPInformation.NetworkAdapter.IanaInterfaceType == 71
                    || hn.IPInformation.NetworkAdapter.IanaInterfaceType == 6))
                {
                    string ipAddress = hn.DisplayName;
                    ipAddresses.Add(ipAddress);
                }
            }

            if (ipAddresses.Count < 1)
            {
                return null;
            }
            else if (ipAddresses.Count == 1)
            {
                return ipAddresses[0];
            }
            else
            {
                //if multiple suitable address were found use the last one
                //(regularly the external interface of an emulated device)
                return ipAddresses[ipAddresses.Count - 1];
            }
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

        public string hostName
        {
            get
            {
                return strHostName;
            }
            set
            {
                strHostName = value;
            }
        }

        #endregion

        #region "Public methods"
        
        public async void Connect(string strHost, int intPort)
        {
            try
            {
                strHostName = strHost;
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
