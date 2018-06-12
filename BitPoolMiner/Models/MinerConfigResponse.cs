using BitPoolMiner.Enums;
using System;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Response used to init miner and start mining
    /// </summary>
    public class MinerConfigResponse
    {
        /// <summary>
        /// Unique account identifier
        /// </summary>
        public Guid AccountGuid { get; set; }

        /// <summary>
        /// Name of worker
        /// </summary>
        public string WorkerName { get; set; }
        /// <summary>
        /// Type of card
        /// </summary>
        public HardwareType HardwareType { get; set; }
        /// <summary>
        /// What Miner should we use per card?
        /// </summary>
        public MinerBaseType MinerBaseType { get; set; }
        /// <summary>
        /// What should this card mine?
        /// </summary>
        public CoinType CoinSelectedForMining { get; set; }
        /// <summary>
        /// If true then this is 64bit software, if false then it is 32bit
        /// </summary>
        public bool X64 { get; set; }
        /// <summary>
        /// String to pass to the miner
        /// </summary>
        public string MinerConfigString { get; set; }
    }
}
