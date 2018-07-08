using System;
using System.Globalization;
using System.Windows;

namespace BitPoolMiner.Models.WhatToMine
{
    public class WhatToMineResponse
    {
        public string name { get; set; }
        public string tag { get; set; }
        public string algorithm { get; set; }
        public string block_time { get; set; }
        public double block_reward { get; set; }
        public double block_reward24 { get; set; }
        public int last_block { get; set; }
        public double difficulty { get; set; }
        public double difficulty24 { get; set; }
        public string nethash { get; set; }
        public double exchange_rate { get; set; }
        public double exchange_rate24 { get; set; }
        public double exchange_rate_vol { get; set; }
        public string exchange_rate_curr { get; set; }
        public string market_cap { get; set; }

        private string estimated_rewards;
        public string Estimated_rewards
        {
            get
            {
                // This is invariant
                NumberFormatInfo format = new NumberFormatInfo();
                // Set the 'splitter' for thousands
                format.NumberGroupSeparator = ",";
                // Set the decimal seperator
                format.NumberDecimalSeparator = ".";

                return Math.Round(Double.Parse(estimated_rewards, format), 6).ToString();
            }
            set
            {
                estimated_rewards = value;
            }
        }

        public string pool_fee { get; set; }

        private string btc_revenue;
        public string BTC_revenue
        {
            get
            {
                // This is invariant
                NumberFormatInfo format = new NumberFormatInfo();
                // Set the 'splitter' for thousands
                format.NumberGroupSeparator = ",";
                // Set the decimal seperator
                format.NumberDecimalSeparator = ".";

                return Math.Round(Double.Parse(btc_revenue, format), 6).ToString();
            }
            set
            {
                btc_revenue = value;
            }
        }

        private string revenue;
        public string Revenue
        {
            get
            {
                return revenue.Replace("$", "");
            }
            set
            {
                revenue = value;
            }            
        }

        public string cost { get; set; }
        public string profit { get; set; }
        public string status { get; set; }
        public bool lagging { get; set; }
        public int timestamp { get; set; }

        /// <summary>
        /// Get the Coin Logo file location
        public string CoinLogo { get; set; }

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
    }
}
