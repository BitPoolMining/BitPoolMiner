using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API.Base;

namespace BitPoolMiner.Persistence.API
{
    /// <summary>
    /// API handler for Account Identity and GUID
    /// </summary>
    class AccountIdentityAPI : APIBase
    {
        /// <summary>
        /// Call API and GET new GUID for the account identity
        /// </summary>
        /// <returns></returns>
        public AccountIdentity GetAccountID()
        {
            string apiURL = APIConstants.APIURL + APIEndpoints.GetAccountGuid;
            AccountIdentity accountIdentity = DownloadSerializedJSONData<AccountIdentity>(apiURL);
            return accountIdentity;
        }
    }
}
