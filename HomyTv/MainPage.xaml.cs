using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Devices.Spi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomyTv
{
    struct HomeStatus
    {
        public byte Mid;
        public byte Light1, Light2, Light3, Light4, Light5;
        public byte Fan1, Fan2;
        public byte Fan1Value, Fan2Value;
    }
    struct BtnPressInfo
    {
        public byte Valid;
        public byte Mid;
        public byte Ep;
        public byte EpType;
        public byte EpSt;
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string SPI_CONTROLLER_NAME = "SPI0";  /* For MinnowBoard Max, use SPI0                            */
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       /* Line 0 maps to physical pin number 5 on the MBM          */

        private byte CurrentMid;
        private uint ColumnCnt;
        private uint RowCnt;
        private string UiPointer;
        private DispatcherTimer timer;
        private SpiDevice SPIRxcver;
        private uint InitDelay = 0;
        private byte[] RxData= { 0, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] TxData = { 0, 0, 0, 0, 0, 0, 0, 0 };
        private byte TxCounter;
        BtnPressInfo BtnPressed;
        public MainPage()
        {
            this.InitializeComponent();
            InitSPIRxcver();
            ColumnCnt = 0;
            RowCnt = 0;
            TxCounter = 0;
            RxData = new byte[8];
            TxData = new byte[8];
            InitDelay = 1;
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(5000);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();            
        }
        private void GetIpcData()
        {
            TxData[0] = BtnPressed.Valid;
            TxData[1] = BtnPressed.Mid;
            TxData[2] = BtnPressed.Ep;
            TxData[3] = BtnPressed.EpType;
            TxData[4] = BtnPressed.EpSt;
            TxData[5] = 0xAA; // BtnPressed.EpSt;
            TxData[6] = 0x55; // BtnPressed.EpSt;
            TxData[7] = TxCounter++; // BtnPressed.EpSt;
            SPIRxcver.TransferFullDuplex(TxData, RxData);
            //SPIRxcver.Write(TxData);
            //SPIRxcver.Read(RxData);
            if (RxData[0] == 0x01)
            {
                // Mid is remote
                if(RxData[1] == 105)
                {
                    ProcessRemote(RxData);
                }
                else
                {

                }

            }
        }
        private void ProcessRemote(byte[] Rx)
        {
            if (RxData[3] == 0x1)
            {
                BtnPressed.Mid = CurrentMid;
                BtnPressed.Ep = 1;
                BtnPressed.EpType = 0;
                BtnPressed.EpSt = 2;
                BtnPressed.Valid = 1;
            }
            else if (RxData[3] == 0x2)
            {
                BtnPressed.Mid = CurrentMid;
                BtnPressed.Ep = 1;
                BtnPressed.EpType = 0;
                BtnPressed.EpSt = 2;
                BtnPressed.Valid = 1;
            }
            else if (RxData[3] == 0x3)
            {
                BtnPressed.Mid = CurrentMid;
                BtnPressed.Ep = 1;
                BtnPressed.EpType = 0;
                BtnPressed.EpSt = 2;
                BtnPressed.Valid = 1;
            }
            else if (RxData[3] == 0x4)
            {
                BtnPressed.Mid = CurrentMid;
                BtnPressed.Ep = 1;
                BtnPressed.EpType = 0;
                BtnPressed.EpSt = 2;
                BtnPressed.Valid = 1;
            }
            else if (RxData[3] == 0x5)
            {
                BtnPressed.Mid = CurrentMid;
                BtnPressed.Ep = 1;
                BtnPressed.EpType = 0;
                BtnPressed.EpSt = 2;
                BtnPressed.Valid = 1;
            }
            else if ((RxData[3] > 0x5) &&(RxData[3] < 0x9))
            {
                UpdateRoom(RxData[1]);
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            if (InitDelay == 1)
            {
                InitDelay = 0;
                this.timer.Interval = TimeSpan.FromMilliseconds(100);
            }
            GetIpcData();
        }
        private void UpdateStatus(HomeStatus HomeSt)
        {
            if (HomeSt.Light1 == 1)
            {
                Tool_11.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            }
            else
            {
                Tool_11.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
            if (HomeSt.Light2 == 1)
            {
                Tool_21.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            }
            else
            {
                Tool_21.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
            if (HomeSt.Light3 == 1)
            {
                Tool_31.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            }
            else
            {
                Tool_31.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
            if (HomeSt.Fan1 == 1)
            {
                Tool_41.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                Tool_53.Text = HomeSt.Fan1Value.ToString();
            }
            else
            {
                Tool_41.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
            if (HomeSt.Fan2 == 1)
            {
                Tool_61.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                Tool_73.Text = HomeSt.Fan1Value.ToString();
            }
            else
            {
                Tool_61.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
        }
        private void UpdateRoom(byte MyRoom)
        {
            if (MyRoom == 0x30) //"Hall"
            {
                CurrentMid = 0x30;
                Tool_10.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                Tool_20.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                Tool_30.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
            else if (MyRoom == 0x31) //"Bedroom"
            {
                CurrentMid = 0x31;
                Tool_10.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                Tool_20.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                Tool_30.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
            }
            else if (MyRoom == 0x32) // "Kitchen")
            {
                CurrentMid = 0x32;
                Tool_10.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                Tool_20.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                Tool_30.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            }
        }
        private void UpdateFocus(uint MyRow, uint MyCol)
        {
            switch (MyRow)
            {
                case 0:
                    Tool_11.Focus(FocusState.Keyboard);
                    break;
                case 1:
                    Tool_21.Focus(FocusState.Keyboard);
                    break;
                case 2:
                    Tool_31.Focus(FocusState.Keyboard);
                    break;
                case 3:
                    Tool_41.Focus(FocusState.Keyboard);
                    break;
                case 5:
                    Tool_61.Focus(FocusState.Keyboard);
                    break;
                case 4:
                    {
                        switch (MyCol)
                        {
                            case 0:
                                {
                                    Tool_51.Focus(FocusState.Keyboard);
                                    break;
                                }
                            case 1:
                                {
                                    Tool_52.Focus(FocusState.Keyboard);
                                    break;
                                }
                        }
                        break;
                    }
                case 6:
                    {
                        switch (MyCol)
                        {
                            case 0:
                                {
                                    Tool_71.Focus(FocusState.Keyboard);
                                    break;
                                }
                            case 1:
                                {
                                    Tool_72.Focus(FocusState.Keyboard);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        private void Tool_10_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Tool_20_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Tool_30_Click(object sender, RoutedEventArgs e)
        {
            // Kitchen

        }

        private void Tool_11_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 1;
            BtnPressed.EpType = 0;
            BtnPressed.EpSt = 0;
            BtnPressed.Valid = 1;
            // Light 1
        }

        private void Tool_21_Click_1(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 2;
            BtnPressed.EpType = 0;
            BtnPressed.EpSt = 0;
            BtnPressed.Valid = 1;

            // Light 2
        }

        private void Tool_31_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 3;
            BtnPressed.EpType = 0;
            BtnPressed.EpSt = 0;
            BtnPressed.Valid = 1;

            // Light 3
        }

        private void Tool_41_Click_1(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 4;
            BtnPressed.EpType = 0;
            BtnPressed.EpSt = 0;
            BtnPressed.Valid = 1;

            // Fan 1
        }

        private void Tool_51_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 4;
            BtnPressed.EpType = 1;
            BtnPressed.EpSt = 1;
            BtnPressed.Valid = 1;

            // Fan 1 +
        }

        private void Tool_53_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 4;
            BtnPressed.EpType = 1;
            BtnPressed.EpSt = 2;
            BtnPressed.Valid = 1;

            // Fan 1 -
        }

        private void Tool_61_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 5;
            BtnPressed.EpType = 0;
            BtnPressed.EpSt = 0;
            BtnPressed.Valid = 1;

            // Fan 2
        }

        private void Tool_71_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 5;
            BtnPressed.EpType = 1;
            BtnPressed.EpSt = 1;
            BtnPressed.Valid = 1;

            // Fan 2 +
        }

        private void Tool_73_Click(object sender, RoutedEventArgs e)
        {
            BtnPressed.Mid = CurrentMid;
            BtnPressed.Ep = 5;
            BtnPressed.EpType = 1;
            BtnPressed.EpSt = 2;
            BtnPressed.Valid = 1;

            // Fan 2 -
        }
        private async void InitSPIRxcver()
        {
            try
            {
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                settings.ClockFrequency = 10000;                              /* 5MHz is the rated speed of the ADXL345 accelerometer                     */
                settings.Mode = SpiMode.Mode3;                                  /* The accelerometer expects an idle-high clock polarity, we use Mode3    
                                                                                 * to set the clock polarity and phase to: CPOL = 1, CPHA = 1         
                settings.                                                                 */

                string aqs = SpiDevice.GetDeviceSelector("SPI0");                     /* Get a selector string that will return all SPI controllers on the system */
                var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the SPI bus controller devices with our selector string             */
                SPIRxcver = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
                if (SPIRxcver == null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
