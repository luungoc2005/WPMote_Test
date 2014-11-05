using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPMote_Desk.Connectivity;
using System.Net.Sockets;

namespace WPMote_Desk.Connectivity
{
    public class Comm_Common
    {
        public static delegate void ConnectedEvent(NetworkStream objRetStream);
        private CommMode objMode;
        NetworkStream objMainStream;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        public void New(CommMode mode)
        {
            objMode = mode;

            switch (mode)
            {
                case CommMode.Bluetooth:
                    Comm_Bluetooth.BluetoothAvailability avail = Comm_Bluetooth.IsBluetoothAvailable();

                    if (avail==Comm_Bluetooth.BluetoothAvailability.NotAvailable)
                    {
                        
                    }
                    else
                    {
                        var objBluetooth=new Comm_Bluetooth();

                        if (avail == Comm_Bluetooth.BluetoothAvailability.TurnedOff) objBluetooth.EnableBluetooth();

                        objBluetooth.Connected += ConnectedHandler;

                        objBluetooth.StartListening();

                        //TODO: Bluetooth - interface handling
                    }

                    break;
                case CommMode.TCP:

                    break;
                default:
                    break;
            }
        }

        public void ConnectedHandler(NetworkStream objStream)
        {
            objMainStream = objStream;
        }
    }
}
