using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;
using System;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for Account Identity and GUID
    /// </summary>
    class AccountIdentityAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Call API and GET new GUID for the account identity
        /// </summary>
        /// <returns></returns>
        public AccountIdentity GetAccountID()
        {
            try
            { 
            string apiURL = APIConstants.APIURL + APIEndpoints.GetAccountGuid;
            AccountIdentity accountIdentity = DownloadSerializedJSONData<AccountIdentity>(apiURL);
            return accountIdentity;
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not download the account idenity information.");
                return new AccountIdentity();
            }
        }
    }
}
