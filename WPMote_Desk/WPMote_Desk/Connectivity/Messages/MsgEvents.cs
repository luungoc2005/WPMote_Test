using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPMote.Connectivity.Messages;

namespace WPMote_Desk.Connectivity.Messages
{
    class MsgEvents
    {
        #region "Common variables"

        public delegate void DClientInfoReceived(Comm_Message.Msg_ClientInfo data);

        #endregion

        #region "Events"

        public event DClientInfoReceived OnClientInfoReceived;

        #endregion

    }
}
