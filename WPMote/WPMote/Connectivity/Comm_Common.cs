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
        private CommMode objMode;
        StreamSocket objMainSocket;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }
        
        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        public void New(CommMode mode)
        {
        }
    }
}
