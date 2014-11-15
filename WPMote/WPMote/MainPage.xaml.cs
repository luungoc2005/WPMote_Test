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
using Microsoft.Devices.Sensors;

namespace WPMote
{
    public partial class MainPage : PhoneApplicationPage
    {
        Comm_Common objComm;
        //Motion objMotion;
        Accelerometer objAccel;
        bool ChkChecked;

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
                objAccel = new Accelerometer();
                objAccel.TimeBetweenUpdates = TimeSpan.FromMilliseconds(100);
                objAccel.CurrentValueChanged += objAccel_CurrentValueChanged;
                objAccel.Start();

                lBtn.AddHandler(UIElement.MouseLeftButtonDownEvent, 
                    new System.Windows.Input.MouseButtonEventHandler(lBtn_MouseLeftButtonDown), true);
                lBtn.AddHandler(UIElement.MouseLeftButtonUpEvent,
                    new System.Windows.Input.MouseButtonEventHandler(lBtn_MouseLeftButtonUp), true);
                rBtn.AddHandler(UIElement.MouseLeftButtonDownEvent,

                    new System.Windows.Input.MouseButtonEventHandler(rBtn_MouseLeftButtonDown), true);
                rBtn.AddHandler(UIElement.MouseLeftButtonUpEvent,
                    new System.Windows.Input.MouseButtonEventHandler(rBtn_MouseLeftButtonUp), true);
            //}
        }
        
        void objAccel_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                txt3.Text = "X:" + e.SensorReading.Acceleration.X + "\r\nY:" + e.SensorReading.Acceleration.Y +
                   "\r\nZ:" + e.SensorReading.Acceleration.Z;
                ChkChecked=(bool)chk1.IsChecked;
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
                        Convert.ToInt16(e.SensorReading.Acceleration.X * 10000),
                        Convert.ToInt16(e.SensorReading.Acceleration.Y * 10000),
                        Convert.ToInt16(e.SensorReading.Acceleration.Z * 10000)).ToByteArray);
                }		 
	        }
        }
        
        private void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                txt3.Text = "Pitch:" + e.SensorReading.Attitude.Pitch + "\r\nYaw:" + e.SensorReading.Attitude.Yaw +
                   "\r\nRoll:" + e.SensorReading.Attitude.Roll;
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            objComm = new Comm_Common(Comm_Common.CommMode.TCP, txt1.Text);
            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
        }

        private void OnClientInfoReceived(string IPAddress, string DeviceName)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            { txt2.Text = DateTime.Now.Ticks + " ClientInfo received: " + IPAddress + " (" + DeviceName + ")"; }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (objComm != null) objComm.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo("127.0.0.1",Microsoft.Phone.Info.DeviceStatus.DeviceName).ToByteArray);
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