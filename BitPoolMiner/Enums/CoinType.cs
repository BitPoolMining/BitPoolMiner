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
            { CoinType.HUSH, @"Images\\hush.png" },
            { CoinType.KMD, @"Images\\kmd.png" },
            { CoinType.VTC, @"Images\\vtc.png" },
            { CoinType.MONA, @"Images\\mona.png" },
            { CoinType.EXP, @"Images\\exp.png" },
        };
    }
}
