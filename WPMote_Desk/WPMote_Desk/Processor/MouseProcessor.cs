using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace WPMote_Desk.Processor
{
    class MouseProcessor
    {
        private const int SamplesCount = 30;

        private const double LowPassFilterCoef = 0.3; // With a 50Hz sampling rate, this is gives a 15Hz cut-off

        private const double NoiseMaxAmplitude = 0.1; // up to 0.1g deviation from filtered value is considered noise

        private Simple3DVector[] _sampleBuffer = new Simple3DVector[SamplesCount];

        private Simple3DVector _previousLowPassOutput;

        private Simple3DVector _previousOptimalFilterOutput;

        private Simple3DVector _sampleSum = new Simple3DVector(0.0 * SamplesCount, 0.0 * SamplesCount, -1.0 * SamplesCount); // assume start flat: -1g in z axis

        private Simple3DVector _averageAcceleration;

        private static double LowPassFilter(double newInputValue, double priorOutputValue)
        {
            double newOutputValue = priorOutputValue + LowPassFilterCoef * (newInputValue - priorOutputValue);
            return newOutputValue;
        }

        private static double FastLowAmplitudeNoiseFilter(double newInputValue, double priorOutputValue)
        {
            double newOutputValue = newInputValue;
            if (Math.Abs(newInputValue - priorOutputValue) <= NoiseMaxAmplitude)
            { // Simple low-pass filter
                newOutputValue = priorOutputValue + LowPassFilterCoef * (newInputValue - priorOutputValue);
            }
            return newOutputValue;
        }

        private bool _initialized = false;

        private int _sampleIndex;
        //0,0: -0.467 -0.6237 -0.6675
        //1,0: 0.5684 -0.5055 -0.6492
        //1,1: 0.5357 0 -0.8444
        //0,1: -0.4219 0 -0.9066

        public static Point AccelToCoord(float X, float Y)
        {
            double rX, rY;
            rX = Math.Max(0,Math.Min(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Width * (Math.Round(X, 3) + 0.5)));
            //rY = Math.Max(0,Math.Min(SystemInformation.WorkingArea.Height,
            //    SystemInformation.WorkingArea.Height * (Y + 0.5)));
            rY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height,
                Screen.PrimaryScreen.Bounds.Height * (Math.Round(Y, 3) + 0.5)));
            return new Point((int)Math.Round(rX), (int)Math.Round(rY));
        }

        public Point AccelToCoordFiltered(float X, float Y, float Z)
        {
            Simple3DVector lowPassFilteredAcceleration = default(Simple3DVector);
            Simple3DVector optimalFilteredAcceleration = default(Simple3DVector);
            Simple3DVector averagedAcceleration = default(Simple3DVector);
            Simple3DVector rawAcceleration = new Simple3DVector(X, Y, Z);
            
            lock (_sampleBuffer)
            {
                if (!_initialized)
                {
                    _sampleSum = rawAcceleration * SamplesCount;
                    _averageAcceleration = rawAcceleration;

                    // Initialize file with 1st value
                    for (int i = 0; i <= SamplesCount - 1; i++)
                    {
                        _sampleBuffer[i] = _averageAcceleration;
                    }

                    _previousLowPassOutput = _averageAcceleration;
                    _previousOptimalFilterOutput = _averageAcceleration;

                    _initialized = true;
                }

                lowPassFilteredAcceleration = new Simple3DVector(LowPassFilter(rawAcceleration.X, _previousLowPassOutput.X), LowPassFilter(rawAcceleration.Y, _previousLowPassOutput.Y), LowPassFilter(rawAcceleration.Z, _previousLowPassOutput.Z));
                _previousLowPassOutput = lowPassFilteredAcceleration;

                optimalFilteredAcceleration = new Simple3DVector(FastLowAmplitudeNoiseFilter(rawAcceleration.X, _previousOptimalFilterOutput.X), FastLowAmplitudeNoiseFilter(rawAcceleration.Y, _previousOptimalFilterOutput.Y), FastLowAmplitudeNoiseFilter(rawAcceleration.Z, _previousOptimalFilterOutput.Z));
                _previousOptimalFilterOutput = optimalFilteredAcceleration;

                _sampleIndex += 1;
                if (_sampleIndex >= SamplesCount) _sampleIndex = 0;

                Simple3DVector newVect = optimalFilteredAcceleration;
                _sampleSum += newVect;
                _sampleSum -= _sampleBuffer[_sampleIndex];
                _sampleBuffer[_sampleIndex] = newVect;

                averagedAcceleration = _sampleSum / SamplesCount;
                _averageAcceleration = averagedAcceleration;
                
                double rX, rY;
                rX = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Width,
                    Screen.PrimaryScreen.Bounds.Width * (Math.Round(optimalFilteredAcceleration.X, 3) + 0.5)));
                //rY = Math.Max(0,Math.Min(SystemInformation.WorkingArea.Height,
                //    SystemInformation.WorkingArea.Height * (Y + 0.5)));
                rY = Math.Max(0, Math.Min(Screen.PrimaryScreen.Bounds.Height,
                    Screen.PrimaryScreen.Bounds.Height * (Screen.PrimaryScreen.Bounds.Width/Screen.PrimaryScreen.Bounds.Height)
                    * (Math.Round(optimalFilteredAcceleration.Y, 3) + 0.5)));
                return new Point((int)Math.Round(rX), (int)Math.Round(rY));
            }

        }
    }
}
