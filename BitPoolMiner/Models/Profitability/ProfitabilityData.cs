using BitPoolMiner.Models.MinerPayments;
using System.Collections.Generic;

namespace BitPoolMiner.Models.Profitability
{
    public class ProfitabilityData
    {
        /// <summary>
        /// List of miner payments by day and unioned across all coins. This list is used to bind to the UI.
        /// </summary>
        public List<MinerPaymentsGroupedByDay> MinerPaymentsGroupedByDayUnionedList { get; set; }
    }
}
