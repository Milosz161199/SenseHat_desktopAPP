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

namespace LedDisplayExample.ViewModel
{
    using Model;

    public class GraphRPYViewModel : INotifyPropertyChanged
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

        public PlotModel DataPlotModel { get; set; }
        public PlotModel DataPlotModel2 { get; set; }
        public PlotModel DataPlotModel3 { get; set; }
        public ButtonCommand StartButton { get; set; }
        public ButtonCommand StopButton { get; set; }
        public ButtonCommand UpdateConfigButton { get; set; }
        public ButtonCommand DefaultConfigButton { get; set; }
        #endregion

        #region Fields
        private int timeStamp = 0;
        private ConfigParams config = new ConfigParams();
        private Timer RequestTimer;
        private IoTServerGraph Server;
        #endregion


        public GraphRPYViewModel()
        {
            // JSON read
            // Create new collection for measurements data
            //Measurements = new ObservableCollection<MeasurementViewModel>();


            DataPlotModel = new PlotModel { Title = "Roll data" };
            DataPlotModel2 = new PlotModel { Title = "Pitch data" };
            DataPlotModel3 = new PlotModel { Title = "Yaw data" };

            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            DataPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 360,
                Key = "Vertical",
                Unit = "Deg",
                Title = "Roll"
            });

            DataPlotModel2.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            DataPlotModel2.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 360,
                Key = "Vertical",
                Unit = "Rad",
                Title = "Pitch"
            });

            DataPlotModel3.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            DataPlotModel3.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 360,
                Key = "Vertical",
                Unit = "Deg",
                Title = "Yaw"
            });

            DataPlotModel.Series.Add(new LineSeries() { Title = "roll data series", Color = OxyColor.Parse("#FFFF0000") });
            DataPlotModel2.Series.Add(new LineSeries() { Title = "pitch data series", Color = OxyColor.Parse("#FFFF0000") });
            DataPlotModel3.Series.Add(new LineSeries() { Title = "yaw data series", Color = OxyColor.Parse("#FFFF0000") });

            StartButton = new ButtonCommand(StartTimer);
            StopButton = new ButtonCommand(StopTimer);
            UpdateConfigButton = new ButtonCommand(UpdateConfig);
            DefaultConfigButton = new ButtonCommand(DefaultConfig);

            ipAddress = config.IpAddress;
            sampleTime = config.SampleTime;

            Server = new IoTServerGraph(IpAddress);
        }

        /**
          * @brief Time series plot update procedure.
          * @param t X axis data: Time stamp [ms].
          * @param d Y axis data: Real-time measurement [-].
          */
        private void UpdatePlot(double t, double d1, double d2, double d3)
        {
            LineSeries lineSeries = DataPlotModel.Series[0] as LineSeries;
            LineSeries lineSeries2 = DataPlotModel2.Series[0] as LineSeries;
            LineSeries lineSeries3 = DataPlotModel3.Series[0] as LineSeries;

            lineSeries.Points.Add(new DataPoint(t, d1));
            lineSeries2.Points.Add(new DataPoint(t, d2));
            lineSeries3.Points.Add(new DataPoint(t, d3));

            if (lineSeries.Points.Count > config.MaxSampleNumber)
                lineSeries.Points.RemoveAt(0);
            if (lineSeries2.Points.Count > config.MaxSampleNumber)
                lineSeries2.Points.RemoveAt(0);
            if (lineSeries3.Points.Count > config.MaxSampleNumber)
                lineSeries3.Points.RemoveAt(0);

            if (t >= config.XAxisMax)
            {
                DataPlotModel.Axes[0].Minimum = (t - config.XAxisMax);
                DataPlotModel.Axes[0].Maximum = t + config.SampleTime / 1000.0;
                DataPlotModel2.Axes[0].Minimum = (t - config.XAxisMax);
                DataPlotModel2.Axes[0].Maximum = t + config.SampleTime / 1000.0;
                DataPlotModel3.Axes[0].Minimum = (t - config.XAxisMax);
                DataPlotModel3.Axes[0].Maximum = t + config.SampleTime / 1000.0;
            }

            DataPlotModel.InvalidatePlot(true);
            DataPlotModel2.InvalidatePlot(true);
            DataPlotModel3.InvalidatePlot(true);
        }

        /**
          * @brief Asynchronous chart update procedure with
          *        data obtained from IoT server responses.
          * @param ip IoT server IP address.
          */
        private async void UpdatePlotWithServerResponse()
        {
#if CLIENT
#if GET
            string RollResponseText = await Server.GET_RollWithClient();
            string PitchResponseText = await Server.GET_PitchWithClient();
            string YawResponseText = await Server.GET_YawWithClient();
#else
            string responseText = await Server.POSTwithClient();
#endif
#else
#if GET
            string responseText = await Server.GETwithRequest();
#else
            string responseText = await Server.POSTwithRequest();
#endif
#endif
            try
            {
#if DYNAMIC
                dynamic resposneJson = JArray.Parse(responseText);
                UpdatePlot(timeStamp / 1000.0, (double)resposneJson.roll, (double)resposneJson.pitch );
#else
                ServerData RollResponseJson = JsonConvert.DeserializeObject<ServerData>(RollResponseText);
                ServerData PitchResponseJson = JsonConvert.DeserializeObject<ServerData>(PitchResponseText);
                ServerData YawResponseJson = JsonConvert.DeserializeObject<ServerData>(YawResponseText);
                UpdatePlot(timeStamp / 1000.0, (double)RollResponseJson.value, (double)PitchResponseJson.value, (double)YawResponseJson.value);
#endif
            }
            catch (Exception e)
            {
                Debug.WriteLine("JSON DATA ERROR");
                Debug.WriteLine(RollResponseText);
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

                DataPlotModel.ResetAllAxes();
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
        private void UpdateConfig()
        {
            bool restartTimer = (RequestTimer != null);

            if (restartTimer)
                StopTimer();

            config = new ConfigParams(ipAddress, sampleTime);
            Server = new IoTServerGraph(IpAddress);

            if (restartTimer)
                StartTimer();
        }

        /**
          * @brief Configuration parameters defualt values
          */
        private void DefaultConfig()
        {
            bool restartTimer = (RequestTimer != null);

            if (restartTimer)
                StopTimer();

            config = new ConfigParams();
            IpAddress = config.IpAddress;
            SampleTime = config.SampleTime.ToString();
            Server = new IoTServerGraph(IpAddress);

            if (restartTimer)
                StartTimer();
        }

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
