using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;


namespace LedDisplayExample.Model
{
    public class IoTServerList
    {
        private string ip;

        public IoTServerList(string _ip)
        {
            ip = _ip;
        }

        /**
         * @brief obtaining the address of the data files from IoT server IP.
         */

        #region URL's for RPY [deg]
        private string GetRollUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/roll_deg.json";
        }
        private string GetPitchUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/pitch_deg.json";
        }
        private string GetYawUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/yaw_deg.json";
        }
        #endregion

        #region URL's for RPY [rad]
        private string GetRollRadUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/roll_rad.json";
        }
        private string GetPitchRadUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/pitch_rad.json";
        }
        private string GetYawRadUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/yaw_rad.json";
        }

        #endregion

        #region URL's for temp in C
        private string GetTemperatureC1Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_C_1.json";
        }
        private string GetTemperatureC2Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_C_2.json";
        }
        private string GetTemperatureC3Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_C_3.json";
        }
        #endregion

        #region URL's for temp in F
        private string GetTemperatureF1Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_F_1.json";
        }
        private string GetTemperatureF2Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_F_2.json";
        }
        private string GetTemperatureF3Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_F_3.json";
        }
        #endregion

        #region URL'S for  RPY accelerometer 
        private string GetRollAccelerometerUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/accelerometer_roll.json";
        }
        private string GetPitchAccelerometerUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/accelerometer_pitch.json";
        }
        private string GetYawAccelerometerUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/accelerometer_yaw.json";
        }

        #endregion

        #region URL'S for  RPY gyroscope 
        private string GetRollGyroscopeUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/gyroscope_roll_deg.json";
        }
        private string GetPitchGyroscopeUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/gyroscope_pitch_deg.json";
        }
        private string GetYawGyroscopeUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/gyroscope_yaw_deg.json";
        }

        #endregion

        #region URL'S for  xyz accelerometer 
        private string GetXAccelerometerUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/accelerometer_x.json";
        }
        private string GetYAccelerometerUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/accelerometer_y.json";
        }
        private string GetZAccelerometerUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/accelerometer_z.json";
        }

        #endregion

        #region URL'S for  xyz gyroscope 
        private string GetXGyroscopeUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/gyroscope_x.json";
        }
        private string GetYGyroscopeUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/gyroscope_y.json";
        }
        private string GetZGyroscopeUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/gyroscope_z.json";
        }

        #endregion

        #region  URL's for Compass
        private string GetCompassNorthUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/compass_north.json";
        }
        private string GetCompassXUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/compass_x.json";
        }
        private string GetCompassYUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/compass_y.json";
        }
        private string GetCompassZUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/compass_z.json";
        }

        #endregion

        #region URL'S for humidity
        private string GetHumidityUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/hum_.json";
        }

        private string GetHumidity2Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/hum_p.json";
        }
        #endregion

        #region URL's for pressure
        private string GetPressureUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/pres_hpa.json";
        }

        private string GetPressure2Url()
        {
            return "http://" + ip + "/PROJECT/OneByOne/pres_mm_hg.json";
        }
        #endregion

        #region URL's for counter
        private string GetCounterXUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/counter_x.json";
        }

        private string GetCounterYUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/counter_y.json";
        }
        private string GetCounterMiddleUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/counter_middle.json";
        }
        #endregion




        /**
         * @brief obtaining the address of the PHP script from IoT server IP.
         */
        private string GetScriptUrl()
        {
            return "http://" + ip + "/client_LAB_10/servermock/chartdata.php";
        }

        private string GetConfigUrl()
        {
            return "http://" + ip + "/PROJECT/config_test_file.json";

        }


        /**
          * @brief HTTP GET request using HttpClient
          */
        public async Task<JArray> GET_ListWithClient()
        {
            string ListResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    ListResponseText = "[";
                    ListResponseText += await client.GetStringAsync(GetRollUrl()) +",";
                    ListResponseText  += await client.GetStringAsync(GetPitchUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetYawUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetRollRadUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetPitchRadUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetYawRadUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetTemperatureC1Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetTemperatureC2Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetTemperatureC3Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetTemperatureF1Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetTemperatureF2Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetTemperatureF3Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetRollAccelerometerUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetPitchAccelerometerUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetYawAccelerometerUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetRollGyroscopeUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetPitchGyroscopeUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetYawGyroscopeUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetXAccelerometerUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetYAccelerometerUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetZAccelerometerUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetXGyroscopeUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetYGyroscopeUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetZGyroscopeUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetHumidityUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetHumidity2Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetPressureUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetPressure2Url()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCounterXUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCounterYUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCounterMiddleUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCompassNorthUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCompassXUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCompassYUrl()) + ",";
                    ListResponseText += await client.GetStringAsync(GetCompassZUrl()) + ",";
                    ListResponseText += "]";
          


                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return JArray.Parse(ListResponseText);

        }

        public async Task<string> GET_ConfigWithClient()
        {
            string ConfigResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    ConfigResponseText = await client.GetStringAsync(GetConfigUrl());

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return ConfigResponseText;

        }

        public async Task<string> GET_JoystickXWithClient()
        {
            string JoystickResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    JoystickResponseText += await client.GetStringAsync(GetCounterXUrl());

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return JoystickResponseText;

        }

        public async Task<string> GET_JoystickYWithClient()
        {
            string JoystickResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    JoystickResponseText += await client.GetStringAsync(GetCounterYUrl());
                    
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return JoystickResponseText;

        }
        public async Task<string> GET_JoystickMiddleWithClient()
        {
            string JoystickResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    JoystickResponseText += await client.GetStringAsync(GetCounterMiddleUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return JoystickResponseText;

        }

        /**
          * @brief HTTP POST request using HttpClient
         */
        public async Task<string> POSTwithClient()
        {
            string responseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // POST request data
                    var requestDataCollection = new List<KeyValuePair<string, string>>();
                    requestDataCollection.Add(new KeyValuePair<string, string>("filename", "chartdata"));
                    var requestData = new FormUrlEncodedContent(requestDataCollection);
                    // Sent POST request
                    var result = await client.PostAsync(GetScriptUrl(), requestData);
                    // Read response content
                    responseText = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return responseText;
        }

        /**
          * @brief HTTP GET request using HttpWebRequest
          */
        public async Task<string> GETwithRequest()
        {
            string responseText = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetRollUrl());

                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseText = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return responseText;
        }

        /**
          * @brief HTTP POST request using HttpWebRequest
          */
        public async Task<string> POSTwithRequest()
        {
            string responseText = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetScriptUrl());

                // POST Request data 
                var requestData = "filename=chartdata";
                byte[] byteArray = Encoding.UTF8.GetBytes(requestData);
                // POST Request configuration
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                // Wrire data to request stream
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseText = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return responseText;
        }
    }
}
