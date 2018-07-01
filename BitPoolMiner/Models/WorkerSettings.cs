using BitPoolMiner.Enums;
using System.Globalization;

namespace BitPoolMiner.Models
{
    public class WorkerSettings
    {
        /// <summary>
        /// The name of the local worker
        /// </summary>
        public string WorkerName { get; set; }

        /// <summary>
        /// Set to true if mining should immediately start at application start
        /// </summary>
        public bool AutoStartMining { get; set; }

        /// <summary>
        /// Region for Miner
        /// </summary>
        public Region Region { get; set; }

        /// <summary>
        /// Currency Code
        /// </summary>
        public CurrencyList Currency { get; set; }
    }
}
