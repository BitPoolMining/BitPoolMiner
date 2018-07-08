using BitPoolMiner.Enums;
using System;
using System.Collections.Generic;

namespace BitPoolMiner.Models.MinerPayments
{
    /// <summary>
    /// Represents miner payment details
    /// </summary>
    public class MinerPaymentSummary
    {
        /// <summary>
        /// Coin type for payment
        /// </summary>
        public CoinType CoinType { get; set; }

        /// <summary>
        /// Wallet address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Get the Coin Logo file location
        public string CoinLogo { get; set; }

        public Decimal RevenueLast24HourUSD { get; set; }
        public Decimal RevenueLast24HourBTC { get; set; }
        public Decimal RevenueLast24HourCoin { get; set; }

        public Decimal RevenueLast7DaysUSD { get; set; }
        public Decimal RevenueLast7DaysCoin { get; set; }

        public Decimal RevenueLast30DaysUSD { get; set; }
        public Decimal RevenueLast30DaysCoin { get; set; }

        /// <summary>
        /// List of payments summarized by day
        /// </summary>
        public List<MinerPaymentsGroupedByDay> MinerPaymentsGroupedByDayList { get; set; }

        /// <summary>
        /// List of all payments in last 24 hours
        /// </summary>
        public List<MinerPaymentDetails24Hours> MinerPaymentDetails24HoursList { get; set; }
    }
}
