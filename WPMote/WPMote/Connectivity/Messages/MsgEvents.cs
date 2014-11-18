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
        public delegate void DCompressedAccelDataReceived(Int16 X, Int16 Y);
        public delegate void DClickReceived(bool RClick, bool LClick);

        #endregion

        #region "Events"

        public event EventHandler OnTestReceived;
        public event DClientInfoReceived OnClientInfoReceived;
        public event DAccelerometerDataReceived OnAccelerometerDataReceived;
        public event DCompressedAccelDataReceived OnCompressedAccelDataReceived;
        public event DClickReceived OnClickReceived;

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
                    if (OnAccelerometerDataReceived != null) OnAccelerometerDataReceived(objMsg150.X, objMsg150.Y, objMsg150.Z, objMsg150.flags);
                    break;

                case 151: //CompressedAccelDataReceived
                    var objMsg151 = new MsgCommon.CompressedAccelData(data);
                    if (OnCompressedAccelDataReceived != null) OnCompressedAccelDataReceived(objMsg151.X, objMsg151.Y);
                    break;

                case 152: //OnClickReceived
                    var objMsg152 = new MsgCommon.ClickReceived(data);
                    if (OnClickReceived != null) OnClickReceived(objMsg152.RClick, objMsg152.LClick);
                    break;

                default:
                    break;
            }
        }

        #endregion

    }
}
