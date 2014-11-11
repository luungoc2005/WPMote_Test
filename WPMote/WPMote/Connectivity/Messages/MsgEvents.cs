using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPMote.Connectivity.Messages;

namespace WPMote.Connectivity.Messages
{
    class MsgEvents
    {
        #region "Common variables"

        public delegate void DClientInfoReceived(string IPAddress, string DeviceName);

        #endregion

        #region "Events"

        public event EventHandler OnTestReceived;
        public event DClientInfoReceived OnClientInfoReceived;

        #endregion

        #region "Public methods"
        
        public void ProcessMessage(int ID, byte[] data)
        {
            switch (ID)
            {
                case 100: //Test
                    if (OnTestReceived != null) OnTestReceived(this, new EventArgs());
                    break;

                case 101: //ClientInfo
                    var objMessage = new MsgCommon.Msg_ClientInfo(data);
                    if (OnClientInfoReceived != null) OnClientInfoReceived(objMessage.IPAddress,objMessage.DeviceName)
                    break;

                default:
                    break;
            }
        }

        #endregion

    }
}
