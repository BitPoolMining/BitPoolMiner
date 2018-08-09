using BitPoolMiner.Models.CoinMarketCap;
using BitPoolMiner.Persistence.API.Base;
using Newtonsoft.Json.Linq;
using System;

namespace BitPoolMiner.Utils.CoinMarketCap
{
    /// <summary>
    /// API handler for CoinMarketCap.com lookup
    /// </summary>
    class CoinMarketCapAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Call API and GET coin data from www.coinmarketcap.com
        /// </summary>
        /// <param name="cryptoCurrenctName"></param>
        /// <param name="fiatCurrencySymbol"></param>
        /// <returns></returns>
        public CoinMarketCapResponse GetCoinMarketCapResponse(string cryptoCurrencyName, string fiatCurrencySymbol)
        {
            try
            {
                // Instatiate response object
                CoinMarketCapResponse coinMarketCapResponse = new CoinMarketCapResponse();
                coinMarketCapResponse.fiat_currency_iso_symbol = fiatCurrencySymbol;

                // Replace spaces with dashes
                cryptoCurrencyName = cryptoCurrencyName.Replace(" ", "-");

                // Call CoinMarketCap and get rates for BTC and Fiat currency specified 
                string apiURL = string.Format(APIConstants.CoinMarketCapAPIURL, cryptoCurrencyName, fiatCurrencySymbol);
                JArray response = DownloadSerializedJSONData<JArray>(apiURL);

                // Manually map CoinMarketCap data since we don't know the exact property name
                foreach (JObject parsedObject in response.Children<JObject>())
                {
                    // Manually map response to object since the property names vary depending on fiat used
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        string propertyName = parsedProperty.Name;

                        if (propertyName.Equals("price_btc"))
                            coinMarketCapResponse.price_btc = (decimal)parsedProperty.Value;

                        if (propertyName.Equals(String.Format("price_{0}", fiatCurrencySymbol.ToLower())))
                            coinMarketCapResponse.price_fiat = (decimal)parsedProperty.Value;

                        if (propertyName.Equals(String.Format("volume_{0}_24h", fiatCurrencySymbol.ToLower())))
                            coinMarketCapResponse.volume_fiat_24h = (decimal)parsedProperty.Value;

                        if (propertyName.Equals(String.Format("market_cap_{0}", fiatCurrencySymbol.ToLower())))
                            coinMarketCapResponse.market_cap_fiat = (decimal)parsedProperty.Value;

                        if (propertyName.Equals("percent_change_24h"))
                            coinMarketCapResponse.percent_change_24h = (decimal)parsedProperty.Value;

                        if (propertyName.Equals("percent_change_7d"))
                            coinMarketCapResponse.percent_change_7d = (decimal)parsedProperty.Value;
                    }
                }
                return coinMarketCapResponse;
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not download coin market cap data.");
                return new CoinMarketCapResponse();
            }
        }
    }

}
