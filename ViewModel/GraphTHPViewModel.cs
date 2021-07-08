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

    public class GraphTHPViewModel : INotifyPropertyChanged
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

        public PlotModel THPPlotModel { get; set; }
        public PlotModel THPPlotModel2 { get; set; }
        public PlotModel THPPlotModel3 { get; set; }
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


        public GraphTHPViewModel()
        {
            // JSON read
            // Create new collection for measurements data
            //Measurements = new ObservableCollection<MeasurementViewModel>();


            THPPlotModel = new PlotModel { Title = "Temperature" };
            THPPlotModel2 = new PlotModel { Title = "Humidity" };
            THPPlotModel3 = new PlotModel { Title = "Pressure" };

            THPPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            THPPlotModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = -30,
                Maximum = 105,
                Key = "Vertical",
                Unit = "C",
                Title = "Temperature"
            });

            THPPlotModel2.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            THPPlotModel2.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 100,
                Key = "Vertical",
                Unit = "%",
                Title = "Humidity"
            });

            THPPlotModel3.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = config.XAxisMax,
                Key = "Horizontal",
                Unit = "sec",
                Title = "Time"
            });
            THPPlotModel3.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Minimum = 260,
                Maximum = 1260,
                Key = "Vertical",
                Unit = "hPa",
                Title = "Pressure"
            });

            THPPlotModel.Series.Add(new LineSeries() { Title = "temperature data series", Color = OxyColor.Parse("#FFFF0000") });
            THPPlotModel2.Series.Add(new LineSeries() { Title = "humidity data series", Color = OxyColor.Parse("#FFFF0000") });
            THPPlotModel3.Series.Add(new LineSeries() { Title = "pressure data series", Color = OxyColor.Parse("#FFFF0000") });

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
            LineSeries lineSeries = THPPlotModel.Series[0] as LineSeries;
            LineSeries lineSeries2 = THPPlotModel2.Series[0] as LineSeries;
            LineSeries lineSeries3 = THPPlotModel3.Series[0] as LineSeries;

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
                THPPlotModel.Axes[0].Minimum = (t - config.XAxisMax);
                THPPlotModel.Axes[0].Maximum = t + config.SampleTime / 1000.0;
                THPPlotModel2.Axes[0].Minimum = (t - config.XAxisMax);
                THPPlotModel2.Axes[0].Maximum = t + config.SampleTime / 1000.0;
                THPPlotModel3.Axes[0].Minimum = (t - config.XAxisMax);
                THPPlotModel3.Axes[0].Maximum = t + config.SampleTime / 1000.0;
            }

            THPPlotModel.InvalidatePlot(true);
            THPPlotModel2.InvalidatePlot(true);
            THPPlotModel3.InvalidatePlot(true);
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
            string TemperatureResponseText = await Server.GET_TemperatureWithClient();
            string HumidityResponseText = await Server.GET_HumidityWithClient();
            string PressureResponseText = await Server.GET_PressureWithClient();
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
                ServerData TemperatureResponseJson = JsonConvert.DeserializeObject<ServerData>(TemperatureResponseText);
                ServerData HumidityResponseJson = JsonConvert.DeserializeObject<ServerData>(HumidityResponseText);
                ServerData PressureResponseJson = JsonConvert.DeserializeObject<ServerData>(PressureResponseText);
                UpdatePlot(timeStamp / 1000.0, (double)TemperatureResponseJson.value, (double)HumidityResponseJson.value*100, (double)PressureResponseJson.value);
#endif

            }
            catch (Exception e)
            {
                Debug.WriteLine("JSON DATA ERROR");
                Debug.WriteLine(TemperatureResponseText);
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

                THPPlotModel.ResetAllAxes();
                THPPlotModel2.ResetAllAxes();
                THPPlotModel3.ResetAllAxes();
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
