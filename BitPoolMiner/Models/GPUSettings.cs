using BitPoolMiner.Enums;
using System;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Represents an individual GPU for a miner
    /// </summary>
    public class GPUSettings
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
        /// ID of individual GPU
        /// </summary>
        public int GPUID { get; set; }

        /// <summary>
        /// Type of card
        /// </summary>
        public HardwareType HardwareType { get; set; }

        /// <summary>
        /// Description of card
        /// </summary>
        public string HardwareName { set; get; }

        /// <summary>
        /// Is this card currently enabled for mining?
        /// </summary>
        public bool EnabledForMining { set; get; }

        /// <summary>
        /// What should this card mine?
        /// </summary>
        public CoinType CoinSelectedForMining { get; set; }

        /// <summary>
        /// What Miner should we use per card?
        /// </summary>
        public MinerBaseType MinerBaseType { get; set; }

        /// <summary>
        /// GPU Fanspeed
        /// </summary>
        public Int16 Fanspeed { get; set; }

        /// <summary>
        /// GPU Power
        /// </summary>
        public Int16 Power { get; set; }
    }
}
