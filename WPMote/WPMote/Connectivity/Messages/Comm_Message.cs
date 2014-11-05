using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WPMote.Connectivity
{
    //TODO: predetermined length for each message type

    abstract class Comm_Message
    {
        public byte intID;
        public int intLength;
        MemoryStream objStream;

        public readonly int Length
        {
            get
            {
                return intLength;
            }
        }

        public readonly Int16 ID
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

    }
}
