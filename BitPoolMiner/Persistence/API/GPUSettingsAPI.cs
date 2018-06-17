using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for GPU Settings
    /// </summary>
    class GPUSettingsAPI : APIBase
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Get GPU Settings from API
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<GPUSettings> GetGPUSettings()
        {
            string apiURL = "";
            try
            {
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("AccountGuid", Application.Current.Properties["AccountID"].ToString());
                nameValueCollection.Add("WorkerName", Application.Current.Properties["WorkerName"].ToString());

                apiURL = APIConstants.APIURL + APIEndpoints.GetGPUSettings;
                ObservableCollection<GPUSettings> gpuSettings = DownloadSerializedJSONData<ObservableCollection<GPUSettings>>(apiURL, nameValueCollection);
                return gpuSettings;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to get GPU settings from {apiURL}");
                return new ObservableCollection<GPUSettings>();
            }
        }

        /// <summary>
        /// Post GPU Settings to API
        /// </summary>
        /// <returns></returns>
        public async void PostGPUSettings(ObservableCollection<GPUSettings> gpuSettingsList)
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.PostGPUSettings;
            try
            {
                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(gpuSettingsList));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(apiURL, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to post GPU settings from {apiURL}");
            }
        }
    }
}
