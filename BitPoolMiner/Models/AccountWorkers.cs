using Newtonsoft.Json;
using System;
using System.Windows;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Represents a worker for a particular miner
    /// </summary>
    public class AccountWorkers
    {
        // <summary>
        /// Unique account identifier
        /// </summary>
        public Guid AccountGuid { get; set; }

        /// <summary>
        /// Name of worker
        /// </summary>
        public string WorkerName { get; set; }

        /// <summary>
        /// Number of minutes since last online
        /// </summary>
        public int MinutesSinceLastMonitored { get; set; }

        /// <summary>
        /// Status of worker
        /// </summary>
        [JsonIgnore]
        public string Status
        {
            get
            {
                if (CountStats == 0 && MinutesSinceLastMonitored == 0)
                {
                    return "never run";
                }
                else if (CountStats > 0 && MinutesSinceLastMonitored <= 5)
                {
                    // If the worker has a monitor record from the last 5 mins then we can assume it is online
                    return "online";
                }
                else if (CountStats > 0 && MinutesSinceLastMonitored > 5)
                {
                    // If the worker has a monitor record older than 5 mins the assume that it is offline
                    return "offline";
                }
                else
                {
                    return "never run";
                }
            }
        }

        /// <summary>
        /// Is Miner Local or Remote
        /// </summary>
        [JsonIgnore]
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

        /// <summary>
        /// Number of stats records found.  0 indicates worker never started
        /// </summary>
        public int CountStats { get; set; }
    }
}
