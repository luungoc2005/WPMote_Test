using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WPMote.Connectivity
{
    //TODO: predetermined length for each message type

    abstract class Comm_Message: IDisposable
    {
        public byte intID;
        public int intLength;
        MemoryStream objStream;

        internal static const int BUFFER_SIZE = 256;

        internal static Dictionary<byte, UInt16> dictMessages = new Dictionary<byte, UInt16> 
        { 
            {100,sizeof(UInt16)}
        };
        
        public readonly int Length
        {
            get
            {
                return intLength;
            }
        }

        public readonly byte ID
        {
            get
            {
                return intID;
            }
        }

        //Constructor
        abstract Comm_Message(byte iID,byte[] data) 
        {
            intID = iID;
            intLength = data.Length;
            
            objStream = new MemoryStream();

            using (var objWrite=new BinaryWriter(objStream))
            {
                objWrite.Write(data, 0, data.Length);
                objWrite.Flush();
            }
        }

        public byte[] ToByteArray()
        {
            byte[] bData;

            using (var objRead=new BinaryReader(objStream))
            {
                objStream.Seek(0,SeekOrigin.Begin);
                bData=objRead.ReadBytes((int)objStream.Length);
            }

            return bData;
        }


        public void Dispose()
        {
            objStream.Dispose();
        }
    }
}
