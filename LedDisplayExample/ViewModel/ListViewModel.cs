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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LedDisplayExample.ViewModel
{
    using Newtonsoft.Json.Linq;
    using Model;

    public class ListViewModel : INotifyPropertyChanged
    {


        #region Data Model
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private double _value;
        public string value
        {
            get
            {
                return _value.ToString("0.0####", CultureInfo.InvariantCulture);
            }
            set
            {
                if (Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double tmp) && _value != tmp)
                {
                    _value = tmp;
                    OnPropertyChanged("Data");
                }
            }
        }

        private string _unit;
        public string unit
        {
            get
            {
                return _unit;
            }
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    OnPropertyChanged("Unit");
                }
            }
        }

        private string _sensor;
        public string sensor
        {
            get
            {
                return _sensor;
            }
            set
            {
                if (_sensor != value)
                {
                    _sensor = value;
                    OnPropertyChanged("Sensor");
                }
            }
        }
        #endregion

        #region Model
        public ListViewModel(ServerListData model)
        {
            UpdateWithModel(model);
        }

        public void UpdateWithModel(ServerListData model)
        {   
            _name = model.name;
            OnPropertyChanged("Name");
            _value = model.value;
            OnPropertyChanged("Data");
            _unit = model.unit;
            OnPropertyChanged("Unit");
            _sensor = model.sensor;
            OnPropertyChanged("Sensor");
        }
        #endregion

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

 
        #endregion

        #region Fields
        private ConfigParams config = new ConfigParams();
        private IoTServerList Server;
        #endregion


        public ListViewModel()
        {
            ipAddress = config.IpAddress;
            sampleTime = config.SampleTime;

            Server = new IoTServerList(IpAddress);
            // Create new collection for measurements data
            Measurements = new ObservableCollection<ListViewModel>();

            // Bind button with action
            Refresh = new ButtonCommand(RefreshHandler);
        }

        /**
          * @brief List of measurements updating procedure
          */

        #region List refresh
        public ObservableCollection<ListViewModel> Measurements { get; set; }
        public ButtonCommand Refresh { get; set; }

        private async void RefreshHandler()
        {

            JArray responseText = await Server.GET_ListWithClient();

            try
            {
                var measurementsList = responseText.ToObject<List<ServerListData>>();
                if (Measurements.Count < measurementsList.Count)
                {
                    foreach (var m in measurementsList)
                        Measurements.Add(new ListViewModel(m));
                }
                // Update existing elements in collection
                else
                {
                    for (int i = 0; i < Measurements.Count; i++)
                        Measurements[i].UpdateWithModel(measurementsList[i]);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("JSON DATA ERROR");
                Debug.WriteLine(responseText);
                Debug.WriteLine(e);
            }

            
        }
        #endregion


        /**
         * @brief Configuration parameters update -------------------------------------------------------------COMENNTED------------
         */

        #region Parameters update
        /*private void UpdateConfig()
        {
            bool restartTimer = (RequestTimer != null);

            if (restartTimer)
                StopTimer();

            config = new ConfigParams(ipAddress, sampleTime);
            Server = new IoTServerList(IpAddress);

            if (restartTimer)
                StartTimer();
        }

        /**
          * @brief Configuration parameters defualt values
          */
        /*
        private void DefaultConfig()
        {
            bool restartTimer = (RequestTimer != null);

            if (restartTimer)
                StopTimer();

            config = new ConfigParams();
            IpAddress = config.IpAddress;
            SampleTime = config.SampleTime.ToString();
            Server = new IoTServerList(IpAddress);

            if (restartTimer)
                StartTimer();
        }
        */
        #endregion


        /**
         * @brief Simple function to trigger event handler
         * @params propertyName Name of ViewModel property as string
         */

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
