using System.Collections.Generic;

namespace BitPoolMiner.Enums
{
    /// <summary>
    /// Supported Coins
    /// </summary>
    public enum CoinType
    {
        UNDEFINED,
        VTC,
        MONA,
        HUSH,
        KMD,
        EXP
    }

    public static class CoinLogos
    {
        public static readonly Dictionary<CoinType, string> CoinLogoDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.HUSH, @"Resources\\Images\\hush.png" },
            { CoinType.KMD, @"Resources\\Images\\kmd.png" },
            { CoinType.VTC, @"Resources\\Images\\vtc.png" },
            { CoinType.MONA, @"Resources\\Images\\mona.png" },
            { CoinType.EXP, @"Resources\\Images\\exp.png" },
        };
    }

    public static class CoinNames
    {
        public static readonly Dictionary<CoinType, string> CoinNameDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.HUSH, "HUSH" },
            { CoinType.KMD, "KOMODO" },
            { CoinType.VTC, "VERTCOIN" },
            { CoinType.MONA, "MONACOIN" },
            { CoinType.EXP, "EXPANSE" },
        };
     }
}
