using System;
using System.IO;
using Newtonsoft.Json;

namespace LedDisplayExample.Model
{
    public class ConfigJSON
    {
        public string ipAddress { get; set; }
        public int sampleTime { get; set; }
        public int maxSample { get; set; }
        public double apiVersion { get; set; }
    }

    public class ConfigParams
    {

        private const string FILE_NAME = "Config.data";
        byte[] data;
        byte[] data2;
        string configJSONstring = " ";
        string configJSONstring2 = " ";

        static readonly string ipAddressDefault = "192.168.0.23";
        public string IpAddress;
        static readonly int sampleTimeDefault = 500;
        public int SampleTime;
        public  int MaxSampleNumber = 100;
        public readonly int MaxSampleNumberDefault = 100;
        public double ApiVersion = 1.0;
        public double XAxisMax
        {
            get
            {
                return MaxSampleNumber * SampleTime / 1000.0;
            }
            private set { }
        }

        public void ReadConfigDataFromFile()
        {
            if (File.Exists(FILE_NAME))
            {
                FileStream fs = new System.IO.FileStream(FILE_NAME, FileMode.Open);
                data = new byte[fs.Length];
                fs.Position = 0;
                fs.Read(data, 0, (int)fs.Length);
                fs.Close();
            }
          /*  configJSONstring = System.Text.Encoding.Default.GetString(data);

            ConfigJSON configJSON = JsonConvert.DeserializeObject<ConfigJSON>(configJSONstring);
            IpAddress = configJSON.ipAddress;
            SampleTime = configJSON.sampleTime;
            MaxSampleNumber = configJSON.maxSample;
            ApiVersion = configJSON.apiVersion;

            Console.WriteLine(configJSONstring);
          */

        }

        public void WriteDataToConfigFile()
        {
            ConfigJSON configJSON = new ConfigJSON()
            {
                ipAddress = IpAddress,
                sampleTime = SampleTime,
                maxSample = MaxSampleNumber,
                apiVersion = ApiVersion
            };

            configJSONstring2 =  JsonConvert.SerializeObject(configJSON);
            Console.WriteLine(configJSONstring2);

            data2 = System.Text.Encoding.UTF8.GetBytes(configJSONstring2);
            
            FileStream fs = new System.IO.FileStream(FILE_NAME, FileMode.Create);
            fs.Write(data2, 0, data2.Length);
            fs.Close();

        }

        public ConfigParams()
        {
            IpAddress = ipAddressDefault;
            SampleTime = sampleTimeDefault;
            MaxSampleNumber = MaxSampleNumberDefault;
        }

        public ConfigParams(string ip, int st)
        {
            IpAddress = ip;
            SampleTime = st;
        }

        public ConfigParams(string ip, int st, int max, double api)
        {
            IpAddress = ip;
            SampleTime = st;
            MaxSampleNumber = max;
            ApiVersion = api;
        }
    }
}
