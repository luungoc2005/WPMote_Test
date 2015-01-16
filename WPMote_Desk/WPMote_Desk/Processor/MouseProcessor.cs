using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WPMote_Desk.Processor
{
    public sealed class MouseProcessor
    {
        #region "Common variables"
        private static volatile MouseProcessor _singletonInstance;

        private static Object _syncRoot = new Object();

        private List<Simple3DVector> readingsQueue = new List<Simple3DVector>();
        #endregion

        #region "Public methods"
        public static MouseProcessor Instance
        {
            get
            {
                if (_singletonInstance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singletonInstance == null)
                        {
                            _singletonInstance = new MouseProcessor();
                        }
                    }
                }
                return _singletonInstance;
            }
        }
        #endregion

        //0,0: -0.467 -0.6237 -0.6675
        //1,0: 0.5684 -0.5055 -0.6492
        //1,1: 0.5357 0 -0.8444
        //0,1: -0.4219 0 -0.9066

        //public static Point AccelToCoord(float X, float Y, float Z)
        //{
        //    double rX, rY;
        //    rX = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Width,
        //        Screen.PrimaryScreen.Bounds.Width * (Math.Round(X, 3) + 0.5)));
        //    //rY = Math.Max(0,Math.Min(SystemInformation.WorkingArea.Height,
        //    //    SystemInformation.WorkingArea.Height * (Y + 0.5)));
        //    rY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height,
        //        Screen.PrimaryScreen.Bounds.Height * (Math.Round(Y, 3) + 0.5)));
        //    return new Point((int)Math.Round(rX), (int)Math.Round(rY));
        //}
    }
}
