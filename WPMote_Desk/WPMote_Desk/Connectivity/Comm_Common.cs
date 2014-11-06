using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        
        Task tskMessages;
        CancellationTokenSource objCancelSource;
        CancellationToken objCancelToken;
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

            objCancelSource = new CancellationTokenSource();
            objCancelToken = objCancelSource.Token;
            tskMessages = Task.Factory.StartNew(() => ReceiveThread(), objCancelSource.Token);

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
                    objCancelSource.Cancel();

                    //Cancel timeout
                    tskMessages.Wait(TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT));

                    objMainStream.Close();
                    objMainStream.Dispose();
                }
            }
            catch 
            { 
            }
        }

        public void ReceiveThread()
        {
            if (objMainStream != null)
            {
                while (true)
                {
                    objCancelToken.ThrowIfCancellationRequested();

                    var objRead = new BinaryReader(objMainStream);

                    try
                    {
                        //MSG type

                        byte intMsgType = objRead.ReadByte();
                        Debug.Print("Byte received {0}", intMsgType);
                        
                        byte[] bData = new byte[Comm_Message.BUFFER_SIZE - 2];
                        bData = objRead.ReadBytes(Comm_Message.BUFFER_SIZE - 2);
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
                        objRead.Dispose();
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
