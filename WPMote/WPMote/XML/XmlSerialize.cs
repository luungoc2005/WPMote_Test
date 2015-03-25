using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.ControlsEx;
using WPMote;

namespace WPMote.XML
{
    class XmlSerialize
    {
        //public static void Serialize(xmlData myObject)
        //{
        //     Insert code to set properties and fields of the object.
        //    XmlSerializer mySerializer = new XmlSerializer(typeof(xmlData));
        //     To write to a file, create a StreamWriter object.
        //    FileStream myWriter = new FileStream("Sample.xml", FileMode.Open);
        //    mySerializer.Serialize(myWriter, myObject);
        //    myWriter.Close();
        //}
        public static XmlData Deserialize()
        {
            XmlData myObject;
            // Construct an instance of the XmlSerializer with the type
            // of object that is being deserialized.
            XmlSerializer mySerializer =
            new XmlSerializer(typeof(XmlData));
            // To read the file, create a FileStream.
            FileStream myFileStream =
            new FileStream("XML/Touhou2.xml", FileMode.Open);
            // Call the Deserialize method and cast to the object type.
            myObject = (XmlData)mySerializer.Deserialize(myFileStream);
            return myObject;
        }
    }
}
