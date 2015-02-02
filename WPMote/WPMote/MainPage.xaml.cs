using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WPMote.Resources;
using WPMote.Connectivity;
using WPMote.Connectivity.Messages;
using Microsoft.Phone.Applications.Common;

namespace WPMote
{
    public partial class MainPage : PhoneApplicationPage
    {
        Comm_Common objComm;
        //Motion objMotion;
        AccelerometerHelper objAccel;
        bool ChkChecked;

        const Int32 MsgInterval = 100;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            //if (Motion.IsSupported)
            //{
            //    objMotion = new Motion();
            //    objMotion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(50);
            //    objMotion.CurrentValueChanged += motion_CurrentValueChanged;
            //    objMotion.Start();
            //}
            //else
            //{
                objAccel = AccelerometerHelper.Instance;
                objAccel.ReadingChanged += objAccel_ReadingChanged;
                objAccel.Active = true;

                lBtn.AddHandler(UIElement.MouseLeftButtonDownEvent, 
                    new System.Windows.Input.MouseButtonEventHandler(lBtn_MouseLeftButtonDown), true);
                lBtn.AddHandler(UIElement.MouseLeftButtonUpEvent,
                    new System.Windows.Input.MouseButtonEventHandler(lBtn_MouseLeftButtonUp), true);
                rBtn.AddHandler(UIElement.MouseLeftButtonDownEvent,
                    new System.Windows.Input.MouseButtonEventHandler(rBtn_MouseLeftButtonDown), true);
                rBtn.AddHandler(UIElement.MouseLeftButtonUpEvent,
                    new System.Windows.Input.MouseButtonEventHandler(rBtn_MouseLeftButtonUp), true);

                AddInputBtn(wBtn, 0x11);
                AddInputBtn(aBtn, 0x1E);
                AddInputBtn(sBtn, 0x1F);
                AddInputBtn(dBtn, 0x20);
                AddInputBtn(qBtn, 0x10);
                AddInputBtn(eBtn, 0x12);
                AddInputBtn(spaceBtn, 0x39);

                AddInputBtn(zBtn, 0x2C);
                AddInputBtn(xBtn, 0x2D);
                AddInputBtn(shiftBtn, 0x2A);

                AddInputBtn(playBtn, 0xA2, true);
                AddInputBtn(volUpBtn, 0x99, true);
                AddInputBtn(volDownBtn, 0xB0, true);
                AddInputBtn(nextBtn, 0xAE, true);

                objComm = new Comm_Common(Comm_Common.CommMode.TCP);
                objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
            //}
        }

        private void AddInputBtn(Button targetButton, byte scanCode, bool extended = false)
        {
            targetButton.AddHandler(UIElement.MouseLeftButtonUpEvent,
                new System.Windows.Input.MouseButtonEventHandler(InputBtn_MouseLeftButtonDown), true);
            targetButton.AddHandler(UIElement.MouseLeftButtonDownEvent,
                new System.Windows.Input.MouseButtonEventHandler(InputBtn_MouseLeftButtonUp), true);
            targetButton.Tag = new inputData(scanCode, extended);
        }

        private struct inputData
        {
            public byte scanCode = 0;
            public bool extended = false;
            public inputData(byte c, bool e = false)
            {
                scanCode = c;
                extended = e;
            }
        }

        private void InputBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                inputData data = (inputData)((Button)sender).Tag;
                objComm.SendBytes(new MsgCommon.KeyBDReceived(data.scanCode, ((Button)sender).IsPressed, data.extended).ToByteArray);
            }
            catch
            {
            }
        }

        private void InputBtn_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                inputData data = (inputData)((Button)sender).Tag;
                objComm.SendBytes(new MsgCommon.KeyBDReceived(data.scanCode, ((Button)sender).IsPressed, data.extended).ToByteArray);
            }
            catch
            {
            }
        }

        int icount;

        void objAccel_ReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                //txt3.Text = "X:" + e.OptimalyFilteredAcceleration.X + "\r\nY:" + e.OptimalyFilteredAcceleration.Y +
                //   "\r\nZ:" + e.OptimalyFilteredAcceleration.Z;
                txt3.Text = objAccel.CanCalibrate(true, true).ToString();
                ChkChecked = (bool)chk1.IsChecked;
            }));
            lock (this)
            {
                if (ChkChecked)
                {
                    //objComm.SendBytes(new MsgCommon.Msg_AccelerometerData(
                    //    e.SensorReading.Acceleration.X,
                    //    e.SensorReading.Acceleration.Y,
                    //    e.SensorReading.Acceleration.Z,
                    //    0).ToByteArray);
                    objComm.SendBytes(new MsgCommon.CompressedAccelData(
                        Convert.ToInt16(e.OptimalyFilteredAcceleration.X * 10000),
                        Convert.ToInt16(e.OptimalyFilteredAcceleration.Y * 10000),
                        Convert.ToInt16(e.OptimalyFilteredAcceleration.Z * 10000)).ToByteArray, true);
                }

                //txt3.Text = objAccel.CanCalibrate(true, true).ToString();
                //ChkChecked = (bool)chk1.IsChecked;
                Dispatcher.BeginInvoke((Action)(() =>
                { ChkChecked = (bool)chk1.IsChecked;
                txt3.Text = "X:" + e.OptimalyFilteredAcceleration.X + "\r\nY:" + e.OptimalyFilteredAcceleration.Y +
"\r\nZ:" + e.OptimalyFilteredAcceleration.Z;
                }));
            }
            icount += 1;
            if (icount>=3)
            {
                lock (this)
                {
                    if (ChkChecked)
                    {
                        //objComm.SendBytes(new MsgCommon.Msg_AccelerometerData(
                        //    e.SensorReading.Acceleration.X,
                        //    e.SensorReading.Acceleration.Y,
                        //    e.SensorReading.Acceleration.Z,
                        //    0).ToByteArray);
                        //objComm.SendBytes(new MsgCommon.CompressedAccelData(
                        //    Convert.ToInt16(e.OptimalyFilteredAcceleration.X * 10000),
                        //    Convert.ToInt16(e.OptimalyFilteredAcceleration.Y * 10000),
                        //    Convert.ToInt16(e.OptimalyFilteredAcceleration.Z * 10000)).ToByteArray, true);
                    }
                }
                icount = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TCPConnect(txt1.Text);
        }

        private void TCPConnect(string host)
        {
            objComm.Connect(host);
        }

        private void OnClientInfoReceived(string IPAddress, string DeviceName)
        {
            if (DeviceName == Microsoft.Phone.Info.DeviceStatus.DeviceName) return;
            Dispatcher.BeginInvoke((Action)(() =>
            { txt2.Text = DateTime.Now.Ticks + " ClientInfo received: " + IPAddress + " (" + DeviceName + ")"; 
            if (MessageBox.Show("Client info received. Attempt connect?", "Connection request", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                { txt1.Text = IPAddress; }));
                TCPConnect(IPAddress);
            }
            }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (objComm != null) objComm.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo(Comm_TCP.LocalIPAddress(),Microsoft.Phone.Info.DeviceStatus.DeviceName).ToByteArray,true,true);
        }

        private void lBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.ClickReceived(rBtn.IsPressed, lBtn.IsPressed).ToByteArray);
        }

        private void lBtn_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.ClickReceived(rBtn.IsPressed, lBtn.IsPressed).ToByteArray);
        }

        private void rBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.ClickReceived(rBtn.IsPressed, lBtn.IsPressed).ToByteArray);
        }

        private void rBtn_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.ClickReceived(rBtn.IsPressed, lBtn.IsPressed).ToByteArray);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            objAccel.Calibrate(true, true);
        }
        
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}