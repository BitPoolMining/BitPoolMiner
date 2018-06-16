using BitPoolMiner.Enums;
using Newtonsoft.Json;
using System;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Represents a wallet for a miner
    /// </summary>
    public class AccountWallet
    {
        public AccountWallet()
        {
            WalletAddress = "";
        }

        /// <summary>
        /// Unique account identifier
        /// </summary>
        public Guid AccountGuid { get; set; }

        /// <summary>
        /// Full coin name
        /// </summary>
        public string CoinName { get; set; }

        /// <summary>
        /// Coin symbol
        /// </summary>
        public string CoinSymbol { get; set; }

        /// <summary>
        /// Internal enum value
        /// </summary>
        public CoinType CoinType { get; set; }

        /// <summary>
        /// Is the coin currently supported by the pool?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Coin algo type enum value
        /// </summary>
        public AlgorithmType AlgorithmType { get; set; }

        /// <summary>
        /// Address for the miner for the specific coin
        /// </summary>
        public string WalletAddress { get; set; }

        /// <summary>
        /// Get the Coin Logo file location
        public string CoinLogo { get; set; }        
    }
}