using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WPMote.Connectivity
{
    class Comm_UDP
    {
        //Self-contained UDP class

        #region "Common variables"

        const int TIMEOUT_MILLISECONDS = 5000;
        const int MAX_BUFFER_SIZE = 256;
        const int DEFAULT_PORT = 8046;

        Socket objSendSocket;
        Socket objRecvSocket;

        Task tskMessages;
        CancellationTokenSource objCancelSource;
        CancellationToken objCancelToken;
        double DEFAULT_TIMEOUT = 500;

        public delegate void DataReceived(byte[] data);

        public event DataReceived OnDataReceived;

        #endregion

        #region "Class constructor"

        public Comm_UDP()
        {
            objSendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            objRecvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        #endregion

        #region "Public methods"

        public void Start()
        {
            objCancelSource = new CancellationTokenSource();
            objCancelToken = objCancelSource.Token;
            tskMessages = Task.Factory.StartNew(() => ReceiveThread(), objCancelSource.Token);
        }

        public void Close()
        {
            objCancelSource.Cancel();
            objSendSocket.Dispose();
            objRecvSocket.Dispose();
        }

        public void SendBytes(string serverName, byte[] buffer)
        {
            if (objSendSocket != null)
            {
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = new DnsEndPoint(serverName, DEFAULT_PORT);
                //socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                //{
                //});
                byte[] data = new byte[MAX_BUFFER_SIZE - 1];
                Array.Copy(buffer, data, Math.Min(MAX_BUFFER_SIZE, buffer.Length));
                socketEventArg.SetBuffer(data, 0, data.Length);
                objSendSocket.SendToAsync(socketEventArg);
            }
        }

        #endregion

        #region "Private methods"

        private void ReceiveThread()
        {
            if (objRecvSocket != null)
            {
                while (true)
                {
                    objCancelToken.ThrowIfCancellationRequested();
                    try
                    {
                        SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                        socketEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Any, DEFAULT_PORT);
                        socketEventArg.SetBuffer(new Byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);
                        socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                        {
                            if (e.SocketError == SocketError.Success)
                            {
                                byte[] data = new byte[MAX_BUFFER_SIZE];
                                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                                OnDataReceived(data);
                            }
                            else
                            {
                                //response = e.SocketError.ToString();
                            }
                        });
                        objRecvSocket.ReceiveFromAsync(socketEventArg);
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
