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

namespace WPMote.Connectivity
{
    class Comm_Common
        //Note: this class also handles closing of sockets
    {
        public delegate void ConnectedEvent(StreamSocket objRetSocket);
        public delegate void MessageReceived(Comm_Message objMessage);

        public Comm_MsgMaster MessagesEvents;

        public event MessageReceived OnMessageReceived;

        private CommMode objMode;
        StreamSocket objMainSocket;

        Comm_Bluetooth objBluetooth;
        Comm_TCP objTCP;

        Task tskMessages;
        CancellationTokenSource objCancelSource;
        CancellationToken objCancelToken;
        double DEFAULT_TIMEOUT = 500;

        public enum CommMode
        {
            Bluetooth,
            TCP,
        }

        public bool IsConnected() { return (objMainSocket == null); }

        public Comm_Common(CommMode mode, string strHost = "127.0.0.1", int intTCPPort = 8019)
        {
            objMode = mode;
            MessagesEvents=new Comm_MsgMaster();

            switch (mode)
            {
                case CommMode.Bluetooth:
                    objBluetooth = new Comm_Bluetooth();

                    objBluetooth.Connected += ConnectedHandler;
                    
                    //TODO: Bluetooth - interface handling

                    break;
                case CommMode.TCP:
                    objTCP = new Comm_TCP();
                    objTCP.Connect(strHost, intTCPPort);

                    break;
                default:
                    break;
            }
        }


        public void ConnectedHandler(StreamSocket objSocket)
        {
            objMainSocket = objSocket;

            objCancelSource=new CancellationTokenSource();
            objCancelToken=objCancelSource.Token;
            tskMessages=Task.Factory.StartNew(() => ReceiveThread(), objCancelSource.Token);            
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

        public async void ReceiveThread()
        {
            if (objMainSocket!=null)
            {
                while (true)
                {
                    objCancelToken.ThrowIfCancellationRequested();

                    var objRead = new DataReader(objMainSocket.InputStream);

                    try
                    {                        
                        //MSG type
                        if (await objRead.LoadAsync(sizeof(byte)) != sizeof(byte))
                        {
                            //Disconnected
                        }

                        byte intMsgType = objRead.ReadByte();
                        uint intLength = await objRead.LoadAsync(Comm_Message.dictMessages[intMsgType]);

                        if (intLength != Comm_Message.dictMessages[intMsgType])
                        {
                            //Disconnected
                        }

                        byte[] bData = new byte[intLength - 1];
                        objRead.ReadBytes(bData);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch
                    {                        
                        throw;
                    }
                    finally
                    {
                        objRead.DetachStream();
                        objRead.Dispose();
                    }
                }
            }
        }

        public async void SendBytes(byte[] buffer)
        {
            if (objMainSocket != null)
            {
                var objWrite = new DataWriter(objMainSocket.OutputStream);

                try
                {
                    objWrite.WriteBytes(buffer);
                    await objWrite.FlushAsync();
                }
                catch
                {                    
                    throw;
                }
                finally
                {
                    objWrite.DetachStream();
                    objWrite.Dispose();
                }
            }
        }
    }
}
