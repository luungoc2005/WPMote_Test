using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using WPMote_Desk.Connectivity;
using System.Net.Sockets;
using System.IO;
using WPMote.Connectivity.Messages;
using System.Diagnostics;

namespace WPMote_Desk.Connectivity
{
    public class Comm_Common
    {
        public delegate void ConnectedEvent(NetworkStream objRetStream);

        private CommMode objMode;
        NetworkStream objMainStream;

        Comm_Bluetooth objBluetooth;
        Comm_TCP objTCP;
        
        BackgroundWorker bwMessages;
        double DEFAULT_TIMEOUT = 500;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        public bool IsConnected() { return (objMainStream == null); }

        public Comm_Common(CommMode mode, int intTCPPort = 8019)
        {
            objMode = mode;

            switch (mode)
            {
                case CommMode.Bluetooth:
                    Comm_Bluetooth.BluetoothAvailability avail = Comm_Bluetooth.IsBluetoothAvailable();

                    if (avail==Comm_Bluetooth.BluetoothAvailability.NotAvailable)
                    {
                        //TODO: Notify user
                    }
                    else
                    {
                        objBluetooth=new Comm_Bluetooth();
                        if (avail == Comm_Bluetooth.BluetoothAvailability.TurnedOff) objBluetooth.EnableBluetooth();
                        objBluetooth.Connected += ConnectedHandler;

                        objBluetooth.StartListen();

                        //TODO: Bluetooth - interface handling
                    }

                    break;
                case CommMode.TCP:
                    objTCP = new Comm_TCP();
                    objTCP.Port = intTCPPort;
                    objTCP.Connected += ConnectedHandler;

                    objTCP.StartListen();

                    break;
                default:
                    break;
            }
        }

        public void ConnectedHandler(NetworkStream objStream)
        {
            objMainStream = objStream;

            bwMessages = new BackgroundWorker();
            bwMessages.DoWork += new DoWorkEventHandler(ReceiveThread);
            bwMessages.RunWorkerAsync();

            Debug.Print("Connected");
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
                    bwMessages.CancelAsync();

                    objMainStream.Close();
                    objMainStream.Dispose();
                }
            }
            catch 
            { 
            }
        }

        private async void ReceiveThread(object sender, DoWorkEventArgs e)
        {
            if (objMainStream != null)
            {
                while (true)
                {
                    try
                    {
                        //MSG type
                        int intMsgType = objMainStream.ReadByte();
                        if (intMsgType > -1)
                        {
                            Debug.Print("Byte received {0}", intMsgType);

                            byte[] bData = new byte[Comm_Message.BUFFER_SIZE - 2];
                            await objMainStream.ReadAsync(bData, 0, Comm_Message.BUFFER_SIZE - 2);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public void SendBytes(byte[] buffer)
        {
            if (objMainStream != null)
            {
                var objWrite = new BinaryWriter(objMainStream);

                try
                {
                    objWrite.Write(buffer);
                    objWrite.Flush();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    objWrite.Dispose();
                }
            }
        }
    }
}
