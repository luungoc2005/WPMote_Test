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

        public List<Simple3DVector> readingsQueue = new List<Simple3DVector>();

        public Simple3DVector currentVelocity = new Simple3DVector();

        private Simple3DVector previousReading = new Simple3DVector();

        private Timer lagTimer = new Timer();

        private int adjustmentInterval = 800;
        private int stableSamplesCount = 0;
        private Simple3DVector maximumStableOffset = new Simple3DVector(0.003, 0.003, 0.003);

        private const int coordinateMulFactor = 9;
        
        long lngPrevious;
        long lngPing;
        public long lngAvgPing;

        const int intSamples = 30;
        long[] arrSamples = new long[intSamples - 1];
        int intCount = 0;
        long lngSum = 0;

        bool _init = false;

        #endregion

        #region "Class constructor"
        public MouseProcessor()
        {
            //Lag timer initialization
            lagTimer.Interval = 20;
            lagTimer.Tick += lagTimer_Tick;
        }
        
        #endregion
        
        #region "Private methods"
        private void lagTimer_Tick(object sender, EventArgs e)
        {
            //Timer for lag compensation
            //ProcessNextReading();
        }

        private void ProcessNextReading()
        {
            if (readingsQueue.Count != 0)
            {
                currentVelocity += readingsQueue[0]; // (1000 / lagTimer.Interval);

                //return velocity to 0 after a certain stable period
                Simple3DVector differenceVector = readingsQueue[0] - previousReading;
                if ((Math.Abs(differenceVector.X) <= Math.Abs(maximumStableOffset.X)) &&
                    (Math.Abs(differenceVector.Y) <= Math.Abs(maximumStableOffset.Y)) &&
                    (Math.Abs(differenceVector.Z) <= Math.Abs(maximumStableOffset.Z)))
                {
                    stableSamplesCount += lagTimer.Interval;
                    if (stableSamplesCount >= adjustmentInterval)
                    {
                        currentVelocity = new Simple3DVector();
                    }
                }
                else
                {
                    stableSamplesCount = 0;
                }
                previousReading = readingsQueue[0];

                readingsQueue.RemoveAt(0);

                //move mouse pointer
                //Win32.MousePointer.Move(new Point((int)currentVelocity.X * coordinateMulFactor, (int)currentVelocity.Y * coordinateMulFactor));
                Point pos = Win32.MousePointer.Position;
                Win32.MousePointer.Position = new Point(((int)currentVelocity.X * coordinateMulFactor) + pos.X,
                                                        ((int)currentVelocity.Y * coordinateMulFactor) + pos.Y);
            }
        }
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

        public void AddReading(Simple3DVector sensorReading)
        {
            readingsQueue.Add(sensorReading);

            if (lngPrevious == 0)
            {
                lngPrevious = DateTime.Now.Ticks;
            }
            else
            {
                lngPing = (DateTime.Now.Ticks - lngPrevious) / TimeSpan.TicksPerMillisecond;

                lngPrevious = DateTime.Now.Ticks;

                lngSum += lngPing;
                lngSum -= arrSamples[intCount];
                arrSamples[intCount] = lngPing;

                intCount += 1;
                if (intCount >= arrSamples.Length)
                {
                    if (_init == false) _init = true;
                    intCount = 0;
                }

                lngAvgPing = (_init) ? (lngSum / intSamples) : (lngSum / intCount);

                if (lngAvgPing > 0)
                {
                    lagTimer.Interval = (int)lngAvgPing;
                    lagTimer.Enabled = true;
                }
                //Debug.Print("Ping {0} ms", lngAvgPing);
            }

            ProcessNextReading();  //no lag scenario
        }

        public void Start()
        {
            lagTimer.Start();
        }

        public void Stop()
        {
            lagTimer.Stop();
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
