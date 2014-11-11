using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WPMote.Connectivity.Messages
{
    //Process of adding a message: Add size to dict->Event->Struct->Modify constructor

    public class MsgCommon : IDisposable
    {
        #region "Common variables"

        internal const int BUFFER_SIZE = 256;

        internal static Dictionary<byte, Int16> dictMessages = new Dictionary<byte, Int16> 
        { 
            {100,sizeof(Int16)}, //TEST CMD
            {101,4*sizeof(byte)+sizeof(Int16)+128} //ClientInfo: IP & DeviceName, DeviceName 128 chars max
        };

        #endregion
        
        #region "Message Structs"

        internal static struct Msg_Test
        {
            public byte ID = 100;
            //Constructors
            public Msg_Test(byte[] bData) { }

            public Msg_Test() { }

            //To byte array
            public readonly byte[] ToByteArray
            {
                get
                {
                    var bData = new byte[dictMessages[ID]];
                    var objStream = new MemoryStream(bData);
                    var objWrite = new BinaryWriter(objStream);

                    try
                    {
                        objWrite.Write(ID);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        objWrite.Flush();
                        objWrite.Dispose();
                        objStream.Dispose();
                    }

                    return bData;
                }
            }
        }

        internal static struct Msg_ClientInfo
        {
            public string IPAddress;
            public string DeviceName;
            
            //Constructors
            public Msg_ClientInfo(byte[] bData)
            {
                var objStream = new MemoryStream(bData);
                var objRead = new BinaryReader(objStream);

                try
                {
                    IPAddress = objRead.ReadByte().ToString() + "." +
                        objRead.ReadByte().ToString() + "." +
                        objRead.ReadByte().ToString() + "." +
                        objRead.ReadByte().ToString();

                    Int16 strLength = objRead.ReadInt16();
                    string strData = Encoding.Unicode.GetString(objRead.ReadBytes(128), 0, strLength);

                    DeviceName = strData;
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

            public Msg_ClientInfo(string strIP, string strName)
            {
                IPAddress = strIP;
                DeviceName = strName;
            }

            //To byte array
            public readonly byte[] ToByteArray
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

                        objWrite.Write((Int16)Math.Min(DeviceName.Length, 128));
                        objWrite.Write(Encoding.Unicode.GetBytes(DeviceName));
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

    }
}
