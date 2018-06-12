using BitPoolMiner.Enums;
using System;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Captures mining statistics used for monitoring a single GPU
    /// </summary>
    public class GPUMonitorStat
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
        /// Current coin being mined
        /// </summary>
        public CoinType CoinType { get; set; }

        /// <summary>
        /// ID of GPU
        /// </summary>
        public int GPUID { get; set; }

        /// <summary>
        /// Type of GPU from enum {UNDEFINED=0,AMD=1,Nvidia=2}
        /// </summary>
        public HardwareType HardwareType { get; set; }

        /// <summary>
        /// Current GPU HashRate
        /// </summary>
        public decimal HashRate { get; set; }

        /// <summary>
        /// Current GPU Fanspeed
        /// </summary>
        public Int16 FanSpeed { get; set; }

        /// <summary>
        /// Current GPU temp
        /// </summary>
        public Int16 Temp { get; set; }

        /// <summary>
        /// Current GPU Power
        /// </summary>
        public Int16 Power { get; set; }

        #region Display Properties

        /// <summary>
        /// ID of GPU
        /// </summary>
        public string DisplayGPUID { get; set; }

        /// <summary>
        /// Name of worker
        /// </summary>
        public string HardwareName { get; set; }

        /// <summary>
        /// Current GPU HashRate
        /// </summary>
        public string DisplayHashRate { get; set; }

        /// <summary>
        /// Current GPU temp
        /// </summary>
        public string DisplayTemp { get; set; }

        /// <summary>
        /// Current GPU temp
        /// </summary>
        public string DisplayFanSpeed { get; set; }

        /// <summary>
        /// Current GPU Power
        /// </summary>
        public string DisplayPower { get; set; }

        #endregion

    }
}
