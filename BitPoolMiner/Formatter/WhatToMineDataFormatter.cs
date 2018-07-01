using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Models.WhatToMine;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace BitPoolMiner.Formatter
{
    public static class WhatToMineDataFormatter
    {
        public static WhatToMineResponse FormatWhatToMineData(WhatToMineResponse whatToMineResponse, CoinType coinType)
        {
            try
            {
                // Update coin logo for each miner
                CoinLogos.CoinLogoDictionary.TryGetValue(coinType, out string logoSourceLocation);
                if (coinType != CoinType.UNDEFINED)
                    whatToMineResponse.CoinLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logoSourceLocation);

                return whatToMineResponse;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error getting coin logo for what to mine data", ex);
            }
        }

    }
}
