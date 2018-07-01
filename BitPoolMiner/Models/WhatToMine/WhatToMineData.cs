using System;
using System.Collections.Generic;
using System.Windows;

namespace BitPoolMiner.Models.WhatToMine
{
    // Object to bind WhatToMine data to UI
    public class WhatToMineData
    {
        public Decimal ForecastNext24HourUSD { get; set; }
        public Decimal ForecastNext24HourBTC { get; set; }
        public Decimal ForecastNext24HourCoin { get; set; }
        
        public Decimal ForecastLast24HourUSD { get; set; }
        public Decimal ForecastLast24HourBTC { get; set; }
        public Decimal ForecastLast24HourCoin { get; set; }
        
        /// <summary>
        /// Current workers fiat currency for conversions
        /// </summary>
        public string FiatCurrencySymbol
        {
            get
            {
                if (Application.Current.Properties["Currency"] == null)
                    return "";
                else
                    return Application.Current.Properties["Currency"].ToString();
            }
        }
        
        public List<WhatToMineResponse> WhatToMineResponseList { get; set; }
    }
}
