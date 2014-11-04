using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Collections.ObjectModel;

namespace WPMote_Desk.Connectivity
{
    public class Comm_Bluetooth
    {
        ObservableCollection<BluetoothDeviceInfo> lstDevices;
        BluetoothClient objClient;
        private static readonly Guid gService = new Guid("04528CB9-6CB3-4713-85A0-9C47D8E283CB");

        Stream objStream;

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
                if (objRadio.Mode == RadioMode.PowerOff) { return BluetoothAvailability.TurnedOff; }
                else { return BluetoothAvailability.Available; }
            }
        }

        //Selecting devices
        public void UpdatePeers()
        {
            try
            {
                if (objClient==null) { objClient = new BluetoothClient(); }

                BluetoothDeviceInfo[] arrPeers = objClient.DiscoverDevices();

                foreach (var objDevice in lstDevices)
                {
                    if (arrPeers.Contains(objDevice)) { lstDevices.Remove(objDevice); }
                }

                foreach (var objDevice in arrPeers)
                {
                    if (!lstDevices.Contains(objDevice)) { lstDevices.Add(objDevice); }
                }

            }
            catch
            {
                
                throw;
            }
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


        public void AcceptDevice(BluetoothDeviceInfo objDevice)
        {
            try
            {
                if (objClient == null) { objClient = new BluetoothClient(); } // just in case
                objClient.Connect(objDevice.DeviceAddress, gService);
                objStream=objClient.GetStream();
            }
            catch
            {                
                throw;
            }

        }

        public void Close()
        {
            if (objClient!=null)
            {
                objClient.Close();
            }
        }
    }
}
