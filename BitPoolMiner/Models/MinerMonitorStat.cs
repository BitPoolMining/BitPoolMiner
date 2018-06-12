using BitPoolMiner.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Captures mining statistics used for monitoring a group of GPU's on a rig per coin type
    /// </summary>
    public class MinerMonitorStat
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
        public Enums.CoinType CoinType { get; set; }

        /// <summary>
        /// Current GPU HashRate
        /// </summary>
        public decimal HashRate { get; set; }

        /// <summary>
        /// Current GPU Power
        /// </summary>
        public Int32 Power { get; set; }

        /// <summary>
        /// List of GPU Monitor stats
        /// </summary>
        public List<GPUMonitorStat> GPUMonitorStatList { get; set; }

        /// <summary>
        /// Get the Coin Logo file location
        public string CoinLogo { get; set; }

        /// <summary>
        /// Data Table used to bind columns in UI for each GPU
        /// </summary>
        public DataTable DataTableGPUData { get; set; }

        /// <summary>
        /// Created date time
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Number of minutes since last online
        /// </summary>
        public int MinutesSinceLastMonitored { get; set; }

        /// <summary>
        /// Status of worker
        /// </summary>
        public string Status { get; set; }

        /// <summary>        /// Number of stats records found.  0 indicates worker never started        /// </summary>        public int CountStats { get; set; }

        /// <summary>
        /// What Miner should we use per card?
        /// </summary>
        public MinerBaseType MinerBaseType { get; set; }

        /// <summary>
        /// Current GPU HashRate
        /// </summary>
        public string DisplayHashRate { get; set; }

        /// <summary>
        /// Number of minutes since last online
        /// </summary>
        public string DisplayMinutesSinceLastMonitored { get; set; }

        /// <summary>        /// Current GPU Power        /// </summary>        public string DisplayPower { get; set; }

        /// <summary>
        /// Is Miner Local or Remote
        /// </summary>
        public string LocalWorker
        {
            get
            {
                if (Application.Current.Properties["WorkerName"] == null)
                    return "unknown";

                if (Application.Current.Properties["WorkerName"].ToString() == this.WorkerName)
                    return "local";

                else
                    return "remote";
            }
        }
    }
}
