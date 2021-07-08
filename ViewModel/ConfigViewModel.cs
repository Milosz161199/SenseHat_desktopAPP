using System;
using System.ComponentModel;
using LedDisplayExample.Model;


namespace LedDisplayExample.ViewModel
{
    public class ConfigViewModel : INotifyPropertyChanged
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
        private int sampleMax;
        public string SampleMax
        {
            get
            {
                return sampleMax.ToString(); ;
            }
            set
            {
                if (Int32.TryParse(value, out int st))
                {
                    if (sampleMax != st)
                    {
                        sampleMax = st;
                        OnPropertyChanged("SampleMax");
                    }
                }
            }
        }
        private double apiversion;
        public string APIversion
        {
            get
            {
                return apiversion.ToString(); ;
            }
            set
            {
                if (Int32.TryParse(value, out int st))
                {
                    if (apiversion != st)
                    {
                        apiversion = st;
                        OnPropertyChanged("APIversion");
                    }
                }
            }
        }


        public ButtonCommand UpdateConfigButton { get; set; }
        public ButtonCommand DefaultConfigButton { get; set; }
        #endregion

        #region Fields
        private ConfigParams config = new ConfigParams();

        #endregion

        public ConfigViewModel()
        {
            UpdateConfigButton = new ButtonCommand(UpdateConfig);
            DefaultConfigButton = new ButtonCommand(DefaultConfig);

            ipAddress = config.IpAddress;
            sampleTime = config.SampleTime;
            sampleMax = config.MaxSampleNumber;
            apiversion = config.ApiVersion;
        }

        
        #region ButtonCommands
        
        /**
         * @brief Configuration parameters update
         */
        private void UpdateConfig()
        {
            config = new ConfigParams(ipAddress, sampleTime, sampleMax, apiversion);
            config.WriteDataToConfigFile();
            config.ReadConfigDataFromFile();
        }

        /**
          * @brief Configuration parameters defualt values
          */
        private void DefaultConfig()
        {
            config = new ConfigParams();
            IpAddress = config.IpAddress;
            SampleTime = config.SampleTime.ToString();
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
