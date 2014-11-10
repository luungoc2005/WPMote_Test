using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;

namespace WPMote_Desk.Connectivity
{
    public class Comm_Bluetooth
    {
        BluetoothListener objServer;
        BluetoothClient objClient;
        private static readonly Guid gService = new Guid("04528CB9-6CB3-4713-85A0-9C47D8E283CB");

        NetworkStream objStream;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

        public enum BluetoothAvailability
        {
            NotAvailable,
            TurnedOff,
            Available
        }

        public static BluetoothAvailability IsBluetoothAvailable()
        {
            var objRadio = BluetoothRadio.PrimaryRadio;

            if (objRadio == null)
            {
                return BluetoothAvailability.NotAvailable;
            }
            else
            {
                if (objRadio.Mode == RadioMode.PowerOff) return BluetoothAvailability.TurnedOff;
                else { return BluetoothAvailability.Available; }
            }
        }

        // Awaiting devices
        public void StartListen()
        {
            try
            {
                if (objServer==null) objServer = new BluetoothListener(gService);
                objServer.Start();
            }
            catch
            {                
                throw;
            }
        }

        public void StopListen()
        {
            try
            {
                if (objServer != null) objServer.Stop();
            }
            catch
            {
                throw;
            }
        }

        public void AcceptDevice()
        {
            objClient = objServer.AcceptBluetoothClient();
            StopListen();

            //SETTINGS

            objStream = objClient.GetStream();

            if (Connected!=null) Connected(objStream);
        }
        
        public void EnableBluetooth()
        {
            var objRadio = BluetoothRadio.PrimaryRadio;

            if (objRadio==null)
            {
                
            }
            else
            {
                if (objRadio.Mode==RadioMode.PowerOff)
                {
                    BluetoothRadio.PrimaryRadio.Mode = RadioMode.Connectable;
                }
            }
        }
        
        public void Close()
        {
            StopListen();

            if (objClient!=null)
            {
                objClient.Close();
            }
        }
    }
}
