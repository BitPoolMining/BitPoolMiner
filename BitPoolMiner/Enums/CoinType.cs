using System.Collections.Generic;
using System.ComponentModel;

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
        EXP,
        ETH,
        ETC,
        BTCP,
        BTG,
        ZEN,
        ZCL
    }

    public static class CoinLogos
    {
        /// <summary>
        /// Coin icon files
        /// </summary>
        public static readonly Dictionary<CoinType, string> CoinLogoDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.HUSH, @"Resources\\Images\\hush.png" },
            { CoinType.KMD, @"Resources\\Images\\kmd.png" },
            { CoinType.VTC, @"Resources\\Images\\vtc.png" },
            { CoinType.MONA, @"Resources\\Images\\mona.png" },
            { CoinType.EXP, @"Resources\\Images\\exp.png" },
            { CoinType.ETH, @"Resources\\Images\\eth.png" },
            { CoinType.ETC, @"Resources\\Images\\etc.png" },
            { CoinType.BTG, @"Resources\\Images\\btg.png" },
            { CoinType.BTCP, @"Resources\\Images\\btcp.png" },
            { CoinType.ZEN, @"Resources\\Images\\zencash.png" },
            { CoinType.ZCL, @"Resources\\Images\\zcl.png" },
        };
    }

    public static class CoinNames
    {
        /// <summary>
        /// Coin full name
        /// </summary>
        public static readonly Dictionary<CoinType, string> CoinNameDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.HUSH, "HUSH" },
            { CoinType.KMD, "KOMODO" },
            { CoinType.VTC, "VERTCOIN" },
            { CoinType.MONA, "MONACOIN" },
            { CoinType.EXP, "EXPANSE" },
            { CoinType.ETH, "ETHEREUM" },
            { CoinType.ETC, "ETHEREUM CLASSIC" },
            { CoinType.BTG, "BITCOIN GOLD" },
            { CoinType.BTCP, "BITCOIN PRIVATE" },
            { CoinType.ZEN, "ZENCASH" },
            { CoinType.ZCL, "ZCLASSIC" },
        };
     }

    public static class CoinWhatToMineIDDictionary
    {
        /// <summary>
        /// Coin ID for WhatToMine
        /// </summary>
        public static readonly Dictionary<CoinType, int> CoinWhatToMineID = new Dictionary<CoinType, int>
        {
            { CoinType.HUSH, 168 },
            { CoinType.KMD, 174 },
            { CoinType.VTC, 5 },
            { CoinType.MONA, 148 },
            { CoinType.EXP, 154 },
            { CoinType.ETH, 151 },
            { CoinType.ETC, 162 },
            { CoinType.BTG, 214 },
            { CoinType.BTCP, 230 },
            { CoinType.ZEN, 185 },
            { CoinType.ZCL, 167 },
        };
    }

    public static class CoinPaymentChartColor
    {
        /// <summary>
        /// Coin icon files
        /// </summary>
        public static readonly Dictionary<CoinType, string> CoinPaymentChartColorDictionary = new Dictionary<CoinType, string>
        {
            { CoinType.HUSH, "#2d99dc" },
            { CoinType.KMD, "#35bda8" },
            { CoinType.VTC, "#86b34d" },
            { CoinType.MONA, "#e66c40" },
            { CoinType.EXP, "#cb3e4b" },
            { CoinType.ETH, "#08a5e1" },
            { CoinType.ETC, "#343286" },
            { CoinType.BTG, "#732c86" },
            { CoinType.BTCP, "#78fd9a" },
            { CoinType.ZEN, "#68fee0" },
            { CoinType.ZCL, "#a072fc" },
        };
    }
}
