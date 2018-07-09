﻿using System.Collections.Generic;
using System.ComponentModel;

namespace BitPoolMiner.Enums
{
    /// <summary>
    /// Supported Coins
    /// </summary>
    public enum CoinType
    {
        [Description("test")]
        UNDEFINED,
        [Description("VTC")]
        VTC,
        [Description("MONA")]
        MONA,
        [Description("HUSH")]
        HUSH,
        [Description("KMD")]
        KMD,
        [Description("EXP")]
        EXP
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
        };
    }
}
