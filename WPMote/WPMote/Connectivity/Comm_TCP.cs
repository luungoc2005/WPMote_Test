using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace WPMote.Connectivity
{
    class Comm_TCP
    {
        int intPort = 8046;

        public int Port
        {
            get
            {
                return intPort;
            }
            set
            {
                if (value > 0) { intPort = value; }
            }
        }

        public bool Connect(string strHost, int intPort)
        {
            try
            {


                return true;
            }
            catch
            {
                return false;
                throw;
            }
        }
    }
}
