using System;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Each unique account will be distinguied by a GUID
    /// </summary>
    public class AccountIdentity
    {
        /// <summary>
        /// Unique GUID for each miner
        /// </summary>
        public Guid AccountGuid { get; set; }
    }
}
