using BitPoolMiner.Enums;
using BitPoolMiner.Formatter;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for Miner and GPU stats
    /// </summary>
    class MinerMonitorStatsAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Call API and GET list of account workers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<MinerMonitorStat> GetMinerMonitorStats()
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.GetMinerMonitorStats;

            try
            {
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("AccountGuid", Application.Current.Properties["AccountID"].ToString());

                ObservableCollection<MinerMonitorStat> minerMonitorStatList = DownloadSerializedJSONData<ObservableCollection<MinerMonitorStat>>(apiURL, nameValueCollection);

                minerMonitorStatList = MinerMonitorStatsFormatter.FormatMinerMonitorStats(minerMonitorStatList);

                return minerMonitorStatList;
            }
            catch (Exception e)
            {
                //throw new ApplicationException("Error loading monitor stats", ex);
                logger.Error(e, $"Error getting miner monitor stats from {apiURL}");
                return new ObservableCollection<MinerMonitorStat>();
            }
        }

        /// <summary>
        /// Post Miner and GPU stats
        /// </summary>
        /// <returns></returns>
        public async void PostMinerMonitorStats(MinerMonitorStat minerMonitorStat)
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.PostMinerMonitorStats;
            try
            {
                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(minerMonitorStat));

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
                logger.Error(e, $"Error posting miner monitor stats to {apiURL}");
            }
        }
    }
}
