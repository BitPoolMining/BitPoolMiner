using BitPoolMiner.Enums;
using System;

namespace BitPoolMiner.Utils
{
    /// <summary>
    /// Format hashrates based on miner type
    /// </summary>
    public static class HashrateFormatter
    {
        /// <summary>
        /// EWBF suffixes
        /// </summary>
        private static readonly string[] ewbfHashrateSuffixList = new string[] { "Zsol/s", "Esol/s", "Psol/s", "Tsol/s", "Gsol/s", "Msol/s", "Ksol/s", "Sol/s" };

        /// <summary>
        /// CCMiner suffixes
        /// </summary>
        private static readonly string[] ccminerHashrateSuffixList = new string[] { "ZH/s", "EH/s", "PH/s", "TH/s", "GH/s", "MH/s", "KH/s", "H/s" };

        #region String Results

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Format(CoinType coinType, decimal bytes)
        {
            //if (coinType == CoinType.HUSH || coinType == CoinType.KMD)
            //    return FormatEWBFHashrate(bytes);
            //else
                return FormatCCMinerHashrate(bytes);
        }

        /// <summary>
        /// Format hashrate into human readable string for EWBF
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatEWBFHashrate(decimal bytes)
        {
            const int scale = 1024;

            decimal max = (decimal)Math.Pow(scale, ewbfHashrateSuffixList.Length - 1);

            foreach (string ewbfHashrateSuffix in ewbfHashrateSuffixList)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), ewbfHashrateSuffix);

                max /= scale;
            }
            return "0";
        }

        /// <summary>
        /// Format hashrate into human readable string for CCMiner
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatCCMinerHashrate(decimal bytes)
        {
            const int scale = 1024;

            decimal max = (decimal)Math.Pow(scale, ccminerHashrateSuffixList.Length - 1);

            foreach (string ccminerHashrateSuffix in ccminerHashrateSuffixList)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), ccminerHashrateSuffix);

                max /= scale;
            }
            return "0";
        }

        #endregion

        #region Double Results

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double FormatNumeric(CoinType coinType, decimal bytes)
        {
            return FormatCCMinerHashrateNumeric(bytes);
        }

        /// <summary>
        /// Format hashrate into human readable string for EWBF
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double FormatEWBFHashrateNumeric(decimal bytes)
        {
            const int scale = 1024;

            decimal max = (decimal)Math.Pow(scale, ewbfHashrateSuffixList.Length - 1);

            foreach (string ewbfHashrateSuffix in ewbfHashrateSuffixList)
            {
                if (bytes > max)
                    return (double)decimal.Divide(bytes, max);

                max /= scale;
            }
            return double.NaN;
        }

        /// <summary>
        /// Format hashrate into human readable string for CCMiner
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double FormatCCMinerHashrateNumeric(decimal bytes)
        {
            const int scale = 1024;

            decimal max = (decimal)Math.Pow(scale, ccminerHashrateSuffixList.Length - 1);

            foreach (string ccminerHashrateSuffix in ccminerHashrateSuffixList)
            {
                if (bytes > max)
                    return (double)decimal.Divide(bytes, max);

                max /= scale;
            }
            return double.NaN;
        }

        #endregion
    }
}
