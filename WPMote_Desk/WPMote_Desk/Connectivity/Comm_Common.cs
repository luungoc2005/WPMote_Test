using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using WPMote_Desk.Connectivity;
using System.Net.Sockets;
using System.IO;
using WPMote_Desk.Connectivity.Messages;
using System.Diagnostics;

namespace WPMote_Desk.Connectivity
{
    public class Comm_Common
    {
        #region "Common variables"

        public MsgEvents Events = new MsgEvents();

        private CommMode objMode;
        NetworkStream objMainStream;

        Comm_Bluetooth objBluetooth;
        Comm_TCP objTCP;
        Comm_UDP objUDP;

        string classTCPHost;
        int classTCPPort;

        BackgroundWorker bwMessages;
        double DEFAULT_TIMEOUT = 500;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        BinaryWriter objWrite;

        #endregion
        
        #region "Events"

        public delegate void ConnectedEvent(NetworkStream objRetStream);
        public delegate void MessageReceived(int ID, byte[] data);

        public event EventHandler OnConnected;
        public event MessageReceived OnMessageReceived;

        #endregion

        #region "Class properties"

        public bool IsConnected() { return (objMainStream == null); }

        #endregion

        #region "Class constructors"

        public Comm_Common(CommMode mode, int intTCPPort = 8019)
        {
            objMode = mode;

            OnMessageReceived += Events.ProcessMessage;

            switch (mode)
            {
                case CommMode.Bluetooth:
                    break;

                case CommMode.TCP:
                    classTCPHost = "127.0.0.1";
                    classTCPPort = intTCPPort;

                    objTCP = new Comm_TCP();
                    objTCP.Port = intTCPPort;
                    objTCP.Connected += ConnectedHandler;
                    
                    objUDP = new Comm_UDP();
                    objUDP.OnDataReceived += objUDP_OnDataReceived;
                    objUDP.Start();
                    
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region "Public methods"

        public void Connect()
        {
            switch (objMode)
            {
                case CommMode.Bluetooth:

                    Comm_Bluetooth.BluetoothAvailability avail = Comm_Bluetooth.IsBluetoothAvailable();

                    if (avail == Comm_Bluetooth.BluetoothAvailability.NotAvailable)
                    {
                        //TODO: Notify user
                    }
                    else
                    {
                        objBluetooth = new Comm_Bluetooth();
                        if (avail == Comm_Bluetooth.BluetoothAvailability.TurnedOff) objBluetooth.EnableBluetooth();
                        objBluetooth.Connected += ConnectedHandler;

                        objBluetooth.StartListen();

                        //TODO: Bluetooth - interface handling
                    }

                    break;

                case CommMode.TCP:
                    objTCP.StartListen();
                    break;

                default:
                    break;
            }
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

                    objWrite.Dispose();

                    objMainStream.Close();
                    objMainStream.Dispose();
                }
            }
            catch
            {
            }
        }

        public void SendBytes(byte[] buffer, bool fastSend = false, bool broadcastSend = false)
        {
            if ((objMode == CommMode.TCP) && fastSend)
            {
                if (broadcastSend == false)
                {
                    objUDP.SendBytes(classTCPHost, buffer);
                }
                else
                {
                    objUDP.SendBytes(System.Net.IPAddress.Broadcast.ToString(), buffer);
                }
            }
            else if (objMainStream != null)
            {
                try
                {
                    objWrite.Write(buffer);
                    objWrite.Flush();
                }
                catch
                {
                    throw;
                }
            }
        }

        #endregion

        #region "Private methods"

        void objUDP_OnDataReceived(byte[] data)
        {
            byte intMsgType = data[0];

            int intLength = MsgCommon.dictMessages[intMsgType];

            byte[] bData = new byte[Math.Max(intLength, 0)];
            Array.Copy(data, 1, bData, 0, intLength);

            OnMessageReceived.Invoke(intMsgType, bData);
        }

        private void ConnectedHandler(NetworkStream objStream)
        {
            objMainStream = objStream;

            bwMessages = new BackgroundWorker();
            bwMessages.DoWork += new DoWorkEventHandler(ReceiveThread);
            bwMessages.RunWorkerAsync();

            objWrite = new BinaryWriter(objMainStream);

            Debug.Print("Connected");

            if (OnConnected != null) OnConnected(this, new EventArgs());
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
                            //Debug.Print("Msg received {0}", intMsgType);

                            int intLength = MsgCommon.dictMessages[(byte)intMsgType];
                            
                            byte[] bData = new byte[Math.Max(intLength-1,0)];

                            if (intLength>0)
                            {
                                await objMainStream.ReadAsync(bData, 0, intLength-1);
                            }
                            
                            OnMessageReceived.Invoke(intMsgType, bData);
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

        #endregion
        
    }
}
