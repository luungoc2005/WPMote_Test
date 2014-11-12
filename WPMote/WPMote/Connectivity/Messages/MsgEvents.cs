using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPMote.Connectivity.Messages;

namespace WPMote.Connectivity.Messages
{
    public class MsgEvents
    {
        #region "Common variables"

        public delegate void DClientInfoReceived(string IPAddress, string DeviceName);
        public delegate void DAccelerometerDataReceived(float X, float Y, float Z, Int32 flags);

        #endregion

        #region "Events"

        public event EventHandler OnTestReceived;
        public event DClientInfoReceived OnClientInfoReceived;
        public event DAccelerometerDataReceived OnAccelerometerDataReceived;

        #endregion

        #region "Class constructors"

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
                    var objMsg101 = new MsgCommon.Msg_ClientInfo(data);
                    if (OnClientInfoReceived != null) OnClientInfoReceived(objMsg101.IPAddress, objMsg101.DeviceName);
                    break;

                case 150: //AccelerometerData
                    var objMsg150 = new MsgCommon.Msg_AccelerometerData(data);
                    if (OnClientInfoReceived != null) OnAccelerometerDataReceived(objMsg150.X, objMsg150.Y, objMsg150.Z, objMsg150.flags);
                    break;

                default:
                    break;
            }
        }

        #endregion

    }
}
