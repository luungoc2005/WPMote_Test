using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WPMote.Connectivity.Messages
{
    //TODO: predetermined length for each message type

    abstract class Comm_Message : IDisposable
    {
        public byte intID;
        MemoryStream objStream;

        internal const int BUFFER_SIZE = 256;

        internal static Dictionary<byte, UInt16> dictMessages = new Dictionary<byte, UInt16> 
        { 
            {100,sizeof(UInt16)}
        };
        
        //Constructor
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

        public Comm_Message()
        {

        }

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
    }
}
