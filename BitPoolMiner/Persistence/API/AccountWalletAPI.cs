using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for Account Wallet Addresses
    /// </summary>
    class AccountWalletAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Call API and GET list of account wallet addresses
        /// </summary>
        /// <returns></returns>
        public List<AccountWallet> GetAccountWalletList()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            string apiURL = "";
            try
            {
                nameValueCollection.Add("AccountGuid", Application.Current.Properties["AccountID"].ToString());

                apiURL = APIConstants.APIURL + APIEndpoints.GetAccountWallet;
                List<AccountWallet> accountWalletList = DownloadSerializedJSONData<List<AccountWallet>>(apiURL, nameValueCollection);

                // Update all AccountGuid properties in case they are returned null when there is no pre-existing wallet addresses saved for this account
                accountWalletList = accountWalletList.Select(c => { c.AccountGuid = (Guid)Application.Current.Properties["AccountID"]; return c; }).ToList();

                // Update coin logo
                foreach (AccountWallet accountWallet in accountWalletList)
                {
                    CoinLogos.CoinLogoDictionary.TryGetValue(accountWallet.CoinType, out string logoSourceLocation);
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logoSourceLocation);
                    if (accountWallet.WalletAddress == null)
                        accountWallet.WalletAddress = "";
                    accountWallet.CoinLogo = path;
                }

                return accountWalletList;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to retrieve the account wallet list for '{Application.Current.Properties["AccountID"].ToString()}' from '{apiURL}'");
                return new List<AccountWallet>();
            }
        }

        /// <summary>
        /// Post Account Wallet addresses
        /// </summary>
        /// <returns></returns>
        public async void PostAccountWalletList(List<AccountWallet> accountWalletList)
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.PostAccountWallet;
            try
            { 
            // Serialize our concrete class into a JSON String
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountWalletList));

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
                logger.Error(e, $"Unable to post the account wallet list to {apiURL}");
            }
        }

        /// <summary>
        /// Delete Account Wallet Addresses
        /// </summary>
        /// <returns></returns>
        public async void DeleteAccountWallet(AccountWallet accountWallet)
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.DeleteAccountWallet;
            try
            {
                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(accountWallet));

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
            catch (Exception e)
            {
                logger.Error(e, $"Unable to delete the account wallet list from {apiURL} for {accountWallet.AccountGuid}");
            }
        }
    }
}
