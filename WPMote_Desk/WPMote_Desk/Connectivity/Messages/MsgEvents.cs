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

        public delegate void DClientInfoReceived(MsgCommon.Msg_ClientInfo data);

        #endregion

        #region "Events"

        public event DClientInfoReceived OnClientInfoReceived;

        #endregion
    }
}
