using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for Miner Config Strings
    /// </summary>
    class MinerConfigStringAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Send Miner Config string request and get responses for all miners that we need to start
        /// </summary>
        /// <returns></returns>
        public List<MinerConfigResponse> GetMinerConfigResponses(MinerConfigRequest minerConfigRequest)
        {
            try
            {
                string apiURL = APIConstants.APIURL + APIEndpoints.GetMinerConfigString;
                List<MinerConfigResponse> minerConfigResponseList = new List<MinerConfigResponse>();

                // Serialize our concrete class into a JSON String
                var stringPayload = JsonConvert.SerializeObject(minerConfigRequest);

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    // Do the actual request and await the response
                    var httpResponse = httpClient.PostAsync(apiURL, httpContent).Result;

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        // Read response and remove extra formatting
                        var responseContent = httpResponse.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1] { '"' });
                        logger.Info(responseContent);
                        minerConfigResponseList = JsonConvert.DeserializeObject<List<MinerConfigResponse>>(responseContent);
                    }
                }

                return minerConfigResponseList;
            }
            catch(Exception e)
            {
                logger.Error(e,"Error retrieving miner coniguration from API.");
                return new List<MinerConfigResponse>(); //return empty list
            }
        }
    }
}
