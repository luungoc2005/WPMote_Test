using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WPMote.Connectivity;
using WPMote.Connectivity.Messages;
using Windows.Phone;
using Microsoft.Devices.Sensors;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WPMote
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Comm_Common objComm;
        Motion objMotion;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            objMotion=new Motion();
            objMotion.TimeBetweenUpdates=TimeSpan.FromMilliseconds(100);
            objMotion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);
            objMotion.Start();
        }

        private async void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            { txt3.Text = e.SensorReading.Attitude.Yaw + " " + e.SensorReading.Attitude.Pitch +
                " " + e.SensorReading.Attitude.Roll; });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            objComm = new Comm_Common(Comm_Common.CommMode.TCP, txt1.Text);
            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
        }

        private async void OnClientInfoReceived(string IPAddress, string DeviceName)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                { txt2.Text = "ClientInfo received: " + IPAddress + " (" + DeviceName + ")"; });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (objComm!=null) objComm.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo("127.0.0.1", "WP8 Device").ToByteArray);
        }
    }
}
