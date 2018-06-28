using Newtonsoft.Json;
using System;

namespace BitPoolMiner.Models.MinerPayments
{
    /// <summary>
    /// Represents miner payment data summarized by day
    /// </summary>
    public class MinerPaymentsGroupedByDay
    {
        /// <summary>
        /// Amount of payment
        /// </summary>
        [JsonProperty("pa")]
        public Decimal PaymentAmount { get; set; }

        /// <summary>
        /// Date of payment
        /// </summary>
        [JsonIgnore]
        public DateTime PaymentDate
        {
            get
            {
                return new DateTime(PaymentDateTicks).ToLocalTime();
            }
        }


        /// <summary>
        /// Date of payment ticks
        /// </summary>
        [JsonProperty("pd")]
        public long PaymentDateTicks { get; set; }
    }
}
