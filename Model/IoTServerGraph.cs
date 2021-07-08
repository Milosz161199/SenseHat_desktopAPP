using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LedDisplayExample.Model
{
    public class IoTServerGraph
    {
        private string ip;

        public IoTServerGraph(string _ip)
        {
            ip = _ip;
        }

        /**
         * @brief obtaining the address of the data files from IoT server IP.
         */
        #region URL's for RPY Graph
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
        #region URL's for THP Graph
        private string GetTemperatureUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/temp_C_1.json";
        }
        private string GetHumidityUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/hum_.json";
        }
        private string GetPressureUrl()
        {
            return "http://" + ip + "/PROJECT/OneByOne/pres_hpa.json";
        }
        #endregion 



        /**
         * @brief obtaining the address of the PHP script from IoT server IP.
         */
        private string GetScriptUrl()
        {
            return "http://" + ip + "/client_LAB_10/servermock/chartdata.php";
        }

        /**
          * @brief HTTP GET request using HttpClient
          */
        #region GET requests for RPY
        public async Task<string> GET_RollWithClient()
        {
            string RollResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    RollResponseText = await client.GetStringAsync(GetRollUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return RollResponseText;
        }
        public async Task<string> GET_YawWithClient()
        {
            string YawResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    YawResponseText = await client.GetStringAsync(GetYawUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return YawResponseText;
        }
        public async Task<string> GET_PitchWithClient()
        {
            string PitchResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    PitchResponseText = await client.GetStringAsync(GetPitchUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return PitchResponseText;
        }
        #endregion
        #region GET requests for THP
        public async Task<string> GET_TemperatureWithClient()
        {
            string TemperatureResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    TemperatureResponseText = await client.GetStringAsync(GetTemperatureUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return TemperatureResponseText;
        }

        public async Task<string> GET_HumidityWithClient()
        {
            string HumidityResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HumidityResponseText = await client.GetStringAsync(GetHumidityUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return HumidityResponseText;
        }

        public async Task<string> GET_PressureWithClient()
        {
            string PressureResponseText = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    PressureResponseText = await client.GetStringAsync(GetPressureUrl());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("NETWORK ERROR");
                Debug.WriteLine(e);
            }

            return PressureResponseText;
        }
        #endregion
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
