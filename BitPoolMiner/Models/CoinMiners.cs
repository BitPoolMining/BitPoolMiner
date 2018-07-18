using BitPoolMiner.Enums;
using System.Collections.Generic;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// List of related Miners per coin
    /// </summary>
    public class CoinMiners
    {
        // Coin Type
        public CoinType CoinType { get; set; }

        // Available miners
        public List<MinerBaseType> MinerBaseTypeList { get; set; }
    }
}
