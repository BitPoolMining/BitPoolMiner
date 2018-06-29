using System;
using System.Collections.Generic;
using System.Windows;

namespace BitPoolMiner.Models.MinerPayments
{
    /// <summary>
    /// Miner payment data used to bind to the UI
    /// </summary>
    public class MinerPaymentsData
    {
        /// <summary>
        /// Summarized revenue for all coins mined
        /// </summary>
        public Decimal RevenueLast24HourUSD { get; set; }

        /// <summary>
        /// Summarized revenue for all coins mined
        /// </summary>
        public Decimal RevenueLast24HourBTC { get; set; }

        /// <summary>
        /// Summarized revenue for all coins mined
        /// </summary>
        public Decimal RevenueLast24HourCoin { get; set; }

        /// <summary>
        /// List of data for each indivual coin mined
        /// </summary>
        public List<MinerPaymentSummary> MinerPaymentSummaryList { get; set; }

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
