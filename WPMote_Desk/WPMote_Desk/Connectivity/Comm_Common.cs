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

        Comm_Bluetooth objBluetooth;
        Comm_TCP objTCP;



        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        public bool IsConnected() { return (objMainStream == null); }

        public Comm_Common(CommMode mode, int intTCPPort = 8046)
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
                        objBluetooth=new Comm_Bluetooth();

                        if (avail == Comm_Bluetooth.BluetoothAvailability.TurnedOff) objBluetooth.EnableBluetooth();

                        objBluetooth.Connected += ConnectedHandler;

                        objBluetooth.StartListening();

                        //TODO: Bluetooth - interface handling
                    }

                    break;
                case CommMode.TCP:
                    objTCP = new Comm_TCP();
                    objTCP.Port = intTCPPort;

                    objTCP.StartListen();

                    break;
                default:
                    break;
            }
        }

        public void ConnectedHandler(NetworkStream objStream)
        {
            objMainStream = objStream;
        }

        public void Close()
        {
            try
            {
                if (!IsConnected()) //Not yet connected
                {
                    if (objMode == CommMode.Bluetooth) objBluetooth.Close();
                    else objTCP.StopListen();
                }
                else
                {
                    objMainStream.Close();
                }
            }
            catch 
            { 
            }
        }
    }
}
