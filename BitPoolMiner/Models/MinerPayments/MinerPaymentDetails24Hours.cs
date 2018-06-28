using Newtonsoft.Json;
using System;

namespace BitPoolMiner.Models.MinerPayments
{
    /// <summary>
    /// Represents miner payment details for past 24 hours
    /// </summary>
    public class MinerPaymentDetails24Hours
    {
        /// <summary>
        /// Amount of payment
        /// </summary>
        [JsonProperty("pa")]
        public Decimal PaymentAmount { get; set; }

        /// <summary>
        /// Date of payment
        /// </summary>
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
