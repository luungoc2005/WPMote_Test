using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using System.IO;
using Windows.Networking.Sockets;

namespace WPMote.Connectivity
{
    class Comm_Bluetooth
    {
        private static readonly Guid gService = Guid.Parse("34B1CF4D-1069-4AD6-89B6-E161D79BE4D8");
        public List<DeviceInformation> lstDevices;
        RfcommDeviceService objClient;
        StreamSocket objSocket;

        public event Connectivity.Comm_Common.ConnectedEvent Connected;

        //Selecting devices
        public async void UpdatePeers()
        {
            try
            {
                DeviceInformationCollection arrPeers;

                arrPeers = await DeviceInformation.FindAllAsync(
                    RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(gService)));

                foreach (var objDevice in lstDevices)
                {
                    if (arrPeers.Contains(objDevice)) lstDevices.Remove(objDevice);
                }

                foreach (var objDevice in arrPeers)
                {
                    if (!lstDevices.Contains(objDevice)) lstDevices.Add(objDevice);
                }
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    //
                }
                throw;
            }
        }

        public async void AcceptDevice(DeviceInformation objDevice)
        {
            objClient = await RfcommDeviceService.FromIdAsync(objDevice.Id);

            if (objClient==null)
            {
                // Notify: Unable to connect
                return;
            }

            lock (this)
            {
                objSocket = new StreamSocket();
            }

            await objSocket.ConnectAsync(objClient.ConnectionHostName, objClient.ConnectionServiceName);

            Connected(objSocket);
        }

        public void Close()
        {

        }
    }
}
