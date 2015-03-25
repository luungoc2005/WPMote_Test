using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.IO;
using System.Windows.Markup;
using System.Threading.Tasks;
using Windows.Storage;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Resources;
using System.Text;
using System.Windows.ControlsEx;
using WPMote.Resources;
using WPMote.Connectivity;
using WPMote.Connectivity.Messages;
using Microsoft.Phone.Applications.Common;
using WPMote.XML;

namespace WPMote
{
    public partial class MainPage : PhoneApplicationPage
    {
        Comm_Common objComm;
        //Motion objMotion;
        AccelerometerHelper objAccel;
        bool ChkChecked;
        const Int32 MsgInterval = 100;
        CheckBox check;

        public TextBlock NewText;
        xamlBinder ContentBinder = new xamlBinder();


        public MainPage()
        {
            InitializeComponent();
            check = chk1;

            #region InitializeCommunication
            objAccel = AccelerometerHelper.Instance;
            objAccel.ReadingChanged += objAccel_ReadingChanged;
            objAccel.Active = true;
            objComm = new Comm_Common(Comm_Common.CommMode.TCP);
            objComm.Events.OnClientInfoReceived += OnClientInfoReceived;
            #endregion

            ContentBinder.Communicator = objComm;
        }

        //#region XML parser
        //private void ReadXML()
        //{

        //    //Process <XAML> tag into string
        //    XElement x = loadXamlFromFile();
        //    IEnumerable<XElement> elList1 = from el in x.Elements("XAML").Elements() select el;
        //    string s_Xaml = "";
        //    foreach (XElement el in elList1)
        //    {
        //        s_Xaml = s_Xaml + el.ToString();
        //    }

        //    //Save new visual content
        //    FrameworkElement NewContent = (FrameworkElement)XamlReader.Load(s_Xaml);

        //    //Process <Info> tags
        //    ModuleName.Text = x.Element("Info").Element("ModuleName").Value;

        //    //Process <Code>.<Object> tags
        //    elList1 = (from el in x.Elements("Code").Elements("Object") select el);
        //    foreach (XElement el in elList1)
        //    {
        //        ContentBinder.assignHandler(el.Element("Name").Value, el.Element("Type").Value, el.Element("Handler").Value, NewContent);
        //    }

        //    //check = (CheckBox)NewContent.FindName("chk1");

        //    //Add visual elements
        //    ContentPanel.Children.Clear();
        //    ContentPanel.Children.Add(NewContent as UIElement); 
        //}
        //#endregion

        //#region xamlLoader
        //private XElement loadXamlFromFile()
        //{
        //    Uri uri = new Uri("XML/Touhou.xml", UriKind.Relative);
        //    StreamResourceInfo strm = Application.GetResourceStream(uri);
        //    XElement x = XElement.Load(strm.Stream);
        //    return x;
        //}

        //#endregion

        int icount;
        #region Establish Communicator
        void objAccel_ReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                //txt3.Text = "X:" + e.OptimalyFilteredAcceleration.X + "\r\nY:" + e.OptimalyFilteredAcceleration.Y +
                //   "\r\nZ:" + e.OptimalyFilteredAcceleration.Z;
                //txt3.Text = objAccel.CanCalibrate(true, true).ToString();
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
                {
                    ChkChecked = (bool)chk1.IsChecked;
    //                txt3.Text = "X:" + e.OptimalyFilteredAcceleration.X + "\r\nY:" + e.OptimalyFilteredAcceleration.Y +
    //"\r\nZ:" + e.OptimalyFilteredAcceleration.Z;
                }));
            }
            icount += 1;
            if (icount >= 3)
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

        private void OnClientInfoReceived(string IPAddress, string DeviceName)
        {
            if (DeviceName == Microsoft.Phone.Info.DeviceStatus.DeviceName) return;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                //txt2.Text = DateTime.Now.Ticks + " ClientInfo received: " + IPAddress + " (" + DeviceName + ")";
                if (MessageBox.Show("Client info received. Attempt connect?", "Connection request", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    { txt1.Text = IPAddress;}));
                    TCPConnect(IPAddress);
                }
            }));
        }

        private void TCPConnect(string host)
        {
            objComm.Connect(host);
        }

        #endregion

        #region Default Handlers
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TCPConnect(txt1.Text);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (objComm != null) objComm.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            objComm.SendBytes(new MsgCommon.Msg_ClientInfo(Comm_TCP.LocalIPAddress(), Microsoft.Phone.Info.DeviceStatus.DeviceName).ToByteArray, true, true);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            objAccel.Calibrate(true, true);
        }
         
        private void Load_XML(object sender, RoutedEventArgs e)
        {
            //ReadXML();
            XmlData data = XmlSerialize.Deserialize();
            FrameworkElement NewContent = (FrameworkElement)XamlReader.Load(Encoding.UTF8.GetString(data.XAML, 0, data.XAML.Length));

            foreach (XmlBindingData xbd in data.data)
            {
                ContentBinder.assignHandler(xbd.name, xbd.type, xbd.handler, NewContent);
            }

            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(NewContent as UIElement); 


        }


        #endregion
    }
}