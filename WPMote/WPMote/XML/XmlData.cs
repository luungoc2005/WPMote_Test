using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPMote.XML
{
    public class XmlData
    {
        public byte[] XAML {get; set;}
        public string ModuleName {get; set;}
        public List<XmlBindingData> data {get; set;}
    }
    public class XmlBindingData
    {
        public string name { get; set; }
        public string type { get; set; }
        public string handler { get; set; }
    }
}
