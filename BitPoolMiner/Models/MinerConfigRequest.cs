using BitPoolMiner.Enums;
using System.Collections.Generic;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Object used to request Miner Configuration string    /// </summary>
    public class MinerConfigRequest
    {
        /// <summary>
        /// Region for the miner
        /// </summary>
        public Region Region { get; set; }
        /// <summary>
        /// List of wallet addresses
        /// </summary>
        public List<AccountWallet> AccountWalletList { get; set; }
        /// <summary>
        /// List of GPU Settings
        /// </summary>
        public List<GPUSettings> GPUSettingsList { get; set; }
    }
}
