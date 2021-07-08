#define CLIENT
#define GET
//#define DYNAMIC

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace LedDisplayExample.ViewModel
{
    using Newtonsoft.Json.Linq;
    using Model;

    public class JoystickViewModel : INotifyPropertyChanged
    {
        #region Properties
        private string ipAddress;
        public string IpAddress
        {
            get
            {
                return ipAddress;
            }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged("IpAddress");
                }
            }
        }
        private int sampleTime;
        public string SampleTime
        {
            get
            {
                return sampleTime.ToString();
            }
            set
            {
                if (Int32.TryParse(value, out int st))
                {
                    if (sampleTime != st)
                    {
                        sampleTime = st;
                        OnPropertyChanged("SampleTime");
                    }
                }
            }
        }

     
        public ButtonCommand StartButton { get; set; }
        public ButtonCommand StopButton { get; set; }
        public ButtonCommand RefreshButton { get; set; }
        #endregion

        #region Fields
        private int timeStamp = 0;
        private ConfigParams config = new ConfigParams();
        private Timer RequestTimer;
        private IoTServerList Server;
        #endregion


        #region Data
        private string _joystickCounter = "0";
        public string JoystickCounter
        {
            get
            {
                return _joystickCounter;
            }
            set
            {
                if (_joystickCounter != value)
                {
                    _joystickCounter = value;
                    OnPropertyChanged("JoystickCounter");
                }
            }
        }
        public PlotModel JoystickPlotModel { get; set; }
        public ButtonCommand StartTrackButton { get; set; }
        public ButtonCommand StopTrackButton { get; set; }
        private uint min = 0;
        private uint max = 25;
        JoystickData joystickData;
        #endregion





        public JoystickViewModel()
        {
            #region Plots init

            JoystickPlotModel = new PlotModel { Title = "Joystick" };
            JoystickPlotModel.Axes.Add(new LinearAxis
            {
                Title = "X",
                Position = AxisPosition.Bottom,
                Minimum = min,
                Maximum = max,
                MajorStep = 1,
                MajorGridlineColor = OxyPlot.OxyColor.Parse("0,0,0"),
                MajorGridlineThickness = 1,
                MajorGridlineStyle = LineStyle.Solid,
                IsZoomEnabled = false,

            });
            JoystickPlotModel.Axes.Add(new LinearAxis
            {
                Title = "Y",
                Position = AxisPosition.Left,
                Minimum = min,
                Maximum = max,
                MajorStep = 1,
                MajorGridlineColor = OxyPlot.OxyColor.Parse("0,0,0"),
                MajorGridlineThickness = 1,
                MajorGridlineStyle = LineStyle.Solid,
                IsZoomEnabled = false,
            });
            JoystickPlotModel.Series.Add(new LineSeries()
            {
                Color = OxyColors.Pink,
                MarkerFill = OxyColors.DeepPink,
                MarkerStroke = OxyColors.HotPink,
                MarkerType = MarkerType.Circle,
                StrokeThickness = 0,
                MarkerSize = 10,
            });

            #endregion
           
          
            StartButton = new ButtonCommand(StartTimer);
            StopButton = new ButtonCommand(StopTimer);
            RefreshButton = new ButtonCommand(UpdatePlotWithServerResponse);
            ipAddress = config.IpAddress;
            sampleTime = config.SampleTime;

            Server = new IoTServerList(IpAddress);
        }

       
        private void UpdatePlot()
        {
            JoystickCounter = joystickData.middle.ToString();
            LineSeries coordinate = JoystickPlotModel.Series[0] as LineSeries;
            try
            {
                coordinate.Points.RemoveAt(0);
            }
            finally
            {
                coordinate.Points.Add(new DataPoint(joystickData.y, joystickData.x));
            }

            JoystickPlotModel.InvalidatePlot(true);
        }
      



        private async void UpdatePlotWithServerResponse()
        {
            string XresponseText = await Server.GET_JoystickXWithClient();
            string YresponseText = await Server.GET_JoystickYWithClient();
            string MiddleresponseText = await Server.GET_JoystickMiddleWithClient();


            try
            {
                ServerData XResponseJson = JsonConvert.DeserializeObject<ServerData>(XresponseText);
                ServerData YResponseJson = JsonConvert.DeserializeObject<ServerData>(YresponseText);
                ServerData MiddleResponseJson = JsonConvert.DeserializeObject<ServerData>(MiddleresponseText);

                joystickData = new JoystickData
                {
                    x = (uint)XResponseJson.value,
                    y = (uint)YResponseJson.value,
                    middle = (int)MiddleResponseJson.value,
                };

                UpdatePlot();


            }
            catch (Exception e)
            {
                Debug.WriteLine("JSON DATA ERROR");
                Debug.WriteLine(XresponseText);
                Debug.WriteLine(e);
            }

            timeStamp += config.SampleTime;
        }

        /**
          * @brief Synchronous procedure for request queries to the IoT server.
          * @param sender Source of the event: RequestTimer.
          * @param e An System.Timers.ElapsedEventArgs object that contains the event data.
          */
        private void RequestTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdatePlotWithServerResponse();
        }

        #region ButtonCommands

        /**
         * @brief RequestTimer start procedure.
         */
        private void StartTimer()
        {
            if (RequestTimer == null)
            {
                RequestTimer = new Timer(config.SampleTime);
                RequestTimer.Elapsed += new ElapsedEventHandler(RequestTimerElapsed);
                RequestTimer.Enabled = true;

                JoystickPlotModel.ResetAllAxes();
                
            }
        }

        /**
         * @brief RequestTimer stop procedure.
         */
        private void StopTimer()
        {
            if (RequestTimer != null)
            {
                RequestTimer.Enabled = false;
                RequestTimer = null;
            }
        }

        /**
         * @brief Configuration parameters update
         */
        #endregion




        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /**
         * @brief Simple function to trigger event handler
         * @params propertyName Name of ViewModel property as string
         */
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
