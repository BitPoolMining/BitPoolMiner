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
        ETC,
        RVN,
        XMR
    }
    
    public static class CoinLogos
    {
        /// <summary>
        /// Coin icon files
        /// </summary>
        public static readonly Dictionary<CoinType, string> CoinLogoDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.VTC, @"Resources\\Images\\vtc.png" },
            { CoinType.ETC, @"Resources\\Images\\etc.png" },
            { CoinType.RVN, @"Resources\\Images\\rvn.png" },
            { CoinType.XMR, @"Resources\\Images\\xmr.png" },
        };
    }

    public static class CoinNames
    {
        /// <summary>
        /// Coin full name
        /// </summary>
        public static readonly Dictionary<CoinType, string> CoinNameDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.VTC, "VERTCOIN" },
            { CoinType.ETC, "ETHEREUM CLASSIC" },
            { CoinType.RVN, "RAVENCOIN" },
            { CoinType.XMR, "MONERO" },
        };
     }

    public static class CoinWhatToMineIDDictionary
    {
        /// <summary>
        /// Coin ID for WhatToMine
        /// </summary>
        public static readonly Dictionary<CoinType, int> CoinWhatToMineID = new Dictionary<CoinType, int>
        {
            { CoinType.VTC, 5 },
            { CoinType.ETC, 162 },
            { CoinType.RVN, 234 },
            { CoinType.XMR, 101 },
        };
    }

    public static class CoinPaymentChartColor
    {
        /// <summary>
        /// Coin icon files
        /// </summary>
        public static readonly Dictionary<CoinType, string> CoinPaymentChartColorDictionary = new Dictionary<CoinType, string>
        {
            //{ CoinType.HUSH, "#2d99dc" },
            //{ CoinType.KMD, "#35bda8" },
            { CoinType.VTC, "#86b34d" },
            //{ CoinType.MONA, "#e66c40" },
            //{ CoinType.EXP, "#cb3e4b" },
            //{ CoinType.ETH, "#08a5e1" },
            { CoinType.ETC, "#343286" },
            //{ CoinType.BTG, "#732c86" },
            //{ CoinType.BTCP, "#78fd9a" },
            //{ CoinType.ZEN, "#68fee0" },
            //{ CoinType.ZCL, "#a072fc" },
            { CoinType.RVN, "#68fee0" },
            { CoinType.XMR, "#35bda8" },
        };
    }
}
