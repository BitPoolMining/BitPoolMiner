using BitPoolMiner.Enums;
using BitPoolMiner.Models.WhatToMine;
using BitPoolMiner.Persistence.API.Base;
using System;
using System.Collections.Specialized;

namespace BitPoolMiner.Utils.WhatToMine
{
    class WhatToMineAPI : APIBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Call WhatToMine API and get forecasted data
        /// </summary>
        /// <returns></returns>
        public WhatToMineResponse GetWhatToMineEstimates(CoinType coinType, NameValueCollection nameValueCollection)
        {
            try
            {
                // Attempt to get crypto coin name
                CoinWhatToMineIDDictionary.CoinWhatToMineID.TryGetValue(coinType, out int cryptoCoinId);

                // Build WhatToMine API URL from Coin ID Dictionary to get expected coin ID
                string apiURL = String.Format(APIConstants.WhatToMineAPIURL, cryptoCoinId);
                WhatToMineResponse whatToMineResponse = DownloadSerializedJSONData<WhatToMineResponse>(apiURL, nameValueCollection);
                return whatToMineResponse;
            }
            catch (Exception e)
            {
                logger.Error(e, "Could not download WhatToMine data.");
                return new WhatToMineResponse();
            }
        }
    }
}
