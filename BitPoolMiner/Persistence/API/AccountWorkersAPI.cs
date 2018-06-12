using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for Account Workers
    /// </summary>
    class AccountWorkersAPI : APIBase
    {
        /// <summary>
        /// Call API and GET list of account workers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<AccountWorkers> GetAccountWorkers()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("AccountGuid", Application.Current.Properties["AccountID"].ToString());

            string apiURL = APIConstants.APIURL + APIEndpoints.GetAccountWorkers;
            ObservableCollection<AccountWorkers> accountWorkerList = DownloadSerializedJSONData<ObservableCollection<AccountWorkers>>(apiURL, nameValueCollection);
            accountWorkerList = new ObservableCollection<AccountWorkers>(accountWorkerList.OrderBy(i => i.WorkerName));
            return accountWorkerList;
        }

        /// <summary>
        /// Post Account Worker
        /// </summary>
        /// <returns></returns>
        public async void PostAccountWorkers(AccountWorkers accountWorkers)
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.PostAccountWorkers;

            // Serialize our concrete class into a JSON String
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountWorkers));

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

        /// <summary>
        /// Delete Account Worker
        /// </summary>
        /// <returns></returns>
        public async void DeleteAccountWorkers(AccountWorkers accountWorkers)
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.DeletetAccountWorkers;

            // Serialize our concrete class into a JSON String
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountWorkers));

            using (var httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = new StringContent(stringPayload, Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(apiURL)
                };
                await httpClient.SendAsync(request);
            }
        }
    }
}
