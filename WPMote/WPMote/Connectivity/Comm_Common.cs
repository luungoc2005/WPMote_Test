using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using WPMote.Connectivity.Messages;
using System.ComponentModel;

namespace WPMote.Connectivity
{
    class Comm_Common
        //Note: this class also handles closing of sockets
    {
        #region "Common variables"

        public MsgEvents Events = new MsgEvents();

        private CommMode objMode;
        StreamSocket objMainSocket;

        Comm_Bluetooth objBluetooth;
        Comm_TCP objTCP;
        Comm_UDP objUDP;

        string classTCPHost;
        int classTCPPort;

        Task tskMessages;
        CancellationTokenSource objCancelSource;
        CancellationToken objCancelToken;
        double DEFAULT_TIMEOUT = 500;

        DataWriter objWrite;
        DataReader objRead;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        #endregion

        #region "Events"

        public delegate void ConnectedEvent(StreamSocket objRetSocket);
        public delegate void MessageReceived(int ID, byte[] data);

        public event EventHandler OnConnected;
        public event MessageReceived OnMessageReceived;

        #endregion

        #region "Class properties"

        public bool IsConnected() { return (objMainSocket == null); }

        #endregion

        #region "Class constructors"
        
        public Comm_Common(CommMode mode, string strHost = "127.0.0.1", int intTCPPort = 8019)
        {
            objMode = mode;

            OnMessageReceived += Events.ProcessMessage;

            switch (mode)
            {
                case CommMode.Bluetooth:

                    break;

                case CommMode.TCP:
                    classTCPHost = strHost;
                    classTCPPort = intTCPPort;

                    objTCP = new Comm_TCP();
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

        public void Connect(string strHost = "127.0.0.1", int intTCPPort = 8019)
        {
            switch (objMode)
            {
                case CommMode.Bluetooth:
                    objBluetooth = new Comm_Bluetooth();

                    objBluetooth.Connected += ConnectedHandler;

                    //TODO: Bluetooth - interface handling
                    break;

                case CommMode.TCP:
                    classTCPHost = strHost;
                    classTCPPort = intTCPPort;

                    objTCP.Connect(strHost, intTCPPort);
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

                }
                else
                {
                    objCancelSource.Cancel();
                    objWrite.Dispose();
                    objRead.Dispose();

                    //Cancel timeout
                    tskMessages.Wait(TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT));

                    objMainSocket.Dispose();
                    objMainSocket = null;
                }
            }
            catch
            {
            }
        }

        public async void SendBytes(byte[] buffer, bool fastSend=false, bool broadcastSend = false)
        {
            if ((objMode==CommMode.TCP) && fastSend)
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
            else if ((objMainSocket != null) & (objWrite != null))
            {
                objWrite.WriteBytes(buffer);

                try
                {
                    await objWrite.StoreAsync();
                }
                catch
                {
                    throw;
                }
            }
        }

        #endregion

        #region "Private methods"

        private void ConnectedHandler(StreamSocket objSocket)
        {
            objMainSocket = objSocket;

            objWrite = new DataWriter(objMainSocket.OutputStream);
            objRead = new DataReader(objMainSocket.InputStream);

            objCancelSource = new CancellationTokenSource();
            objCancelToken = objCancelSource.Token;
            tskMessages = Task.Factory.StartNew(() => ReceiveThread(), objCancelSource.Token);
            
            if (OnConnected != null) OnConnected(this, new EventArgs());
        }
        
        void objUDP_OnDataReceived(byte[] data)
        {
            byte intMsgType = data[0];

            int intLength = MsgCommon.dictMessages[intMsgType];

            byte[] bData = new byte[Math.Max(intLength, 0)];
            Array.Copy(data, 1, bData, 0, intLength);

            OnMessageReceived.Invoke(intMsgType, bData);
        }

        private async void ReceiveThread()
        {
            if (objMainSocket != null)
            {
                while (true)
                {
                    objCancelToken.ThrowIfCancellationRequested();

                    try
                    {
                        //MSG type
                        if (await objRead.LoadAsync(sizeof(byte)) != sizeof(byte))
                        {
                            //Disconnected
                        }

                        byte intMsgType = objRead.ReadByte();                        

                        int intLength = MsgCommon.dictMessages[intMsgType];
                        
                        byte[] bData = new byte[Math.Max(intLength-1,0)];

                        if (intLength > 0)
                        {
                            await objRead.LoadAsync((uint)intLength);

                            objRead.ReadBytes(bData);
                        }

                        OnMessageReceived.Invoke(intMsgType, bData);
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
