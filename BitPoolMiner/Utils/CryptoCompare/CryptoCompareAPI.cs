using BitPoolMiner.Models.CoinMarketCap;
using BitPoolMiner.Models.CryptoCompare;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json.Linq;
using System;

namespace BitPoolMiner.Utils.CryptoCompare
{
    /// <summary>
    /// API handler for CryptoCompare.com lookup
    /// </summary>
    public class CryptoCompareAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Call API and GET coin data from www.cryptocompare.com
        /// </summary>
        /// <param name="cryptoCurrenctName"></param>
        /// <param name="fiatCurrencySymbol"></param>
        /// <returns></returns>
        public HistoDayResponse GetCryptoCompareResponse(string cryptoCurrencyName, string fiatCurrencySymbol)
        {
            try
            {
                // Instatiate response object
                HistoDayResponse histoDayResponse = new HistoDayResponse();

                // Call CryptoCompare and get daily historical rates
                string apiURL = string.Format(APIConstants.CryptoCompareHistoDayAPIURL, cryptoCurrencyName, fiatCurrencySymbol);
                histoDayResponse = DownloadSerializedJSONData<HistoDayResponse>(apiURL);

                return histoDayResponse;
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not download crypto compare data.");
                return new HistoDayResponse();
            }
        }
    }
}
