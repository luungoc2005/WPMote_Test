using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Networking;
using System.Text;

namespace WPMote.Connectivity.Messages
{
    //TODO: predetermined length for each message type

    abstract class Comm_Message : IDisposable
    {
        #region "Common variables"

        public byte intID;
        MemoryStream objStream;

        internal const int BUFFER_SIZE = 256;

        internal static Dictionary<byte, UInt16> dictMessages = new Dictionary<byte, UInt16> 
        { 
            {100,sizeof(Int16)}, //TEST CMD
            {101,4*sizeof(byte)+sizeof(Int16)+128} //ClientInfo: IP & HostName, MachineName 128 chars max
        };

        #endregion

        #region "Events"



        #endregion

        #region "Message Structs"

        struct Msg_ClientInfo
        {
            byte ID = 101;
            string IPAddress;
            string MachineName;

            //Constructors
            void Msg_ClientInfo(byte[] bData)
            {
                var objStream = new MemoryStream(bData);
                var objRead = new BinaryReader(objStream);

                try
                {
                    IPAddress=objRead.ReadByte().ToString() + "." +
                        objRead.ReadByte().ToString() + "." +
                        objRead.ReadByte().ToString() + "." +
                        objRead.ReadByte().ToString();

                    Int16 strLength = objRead.ReadInt16();
                    string strData = Encoding.Unicode.GetString(objRead.ReadBytes(128),0,strLength);
                    
                    MachineName = strData;
                }
                catch
                {                    
                    throw;
                }
                finally
                {
                    objRead.Dispose();
                    objStream.Dispose();
                }
            }

            void Msg_ClientInfo(string strIP, string strName)
            {
                IPAddress = strIP;
                MachineName = strName;
            }

            //To byte array
            readonly byte[] ToByteArray
            {
                get
                {
                    var bData = new byte[dictMessages[ID]];
                    var objStream = new MemoryStream(bData);
                    var objWrite = new BinaryWriter(objStream);

                    try
                    {
                        //IP Address to byte()
                        string[] strIPTemp = IPAddress.Split('.');

                        if (strIPTemp.Length != 4) throw new Exception("Invalid IP Address");
                        foreach (var temp in strIPTemp)
                        {
                            objWrite.Write(Convert.ToByte(temp));
                        }

                        objWrite.Write((Int16)Math.Min(MachineName.Length, 128));
                        objWrite.Write(Encoding.Unicode.GetBytes(MachineName));
                        objWrite.Flush();
                    }
                    catch
                    {                        
                        throw;
                    }
                    finally
                    {
                        objWrite.Dispose();
                        objStream.Dispose();
                    }

                    return bData;
                }
            }
        }

        #endregion

        #region "Class constructors"

        public Comm_Message(byte iID, byte[] data)
        {
            intID = iID;

            objStream = new MemoryStream();

            using (var objWrite = new BinaryWriter(objStream))
            {
                objWrite.Write(data, 0, data.Length);
                objWrite.Flush();
            }
        }

        //Empty constructor
        public Comm_Message() { }

        #endregion

        #region "Public methods"

        public byte[] ToByteArray()
        {
            byte[] bData;

            using (var objRead = new BinaryReader(objStream))
            {
                objStream.Seek(0, SeekOrigin.Begin);
                bData = objRead.ReadBytes((int)objStream.Length);
            }

            return bData;
        }

        public void Dispose()
        {
            objStream.Dispose();
        }

        #endregion
        
    }
}
