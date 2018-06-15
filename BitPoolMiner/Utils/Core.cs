using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Utils
{
    public static class Core
    {
        /// <summary>
        /// Returns the base directory of the BitPoolMiner executing assembly.
        /// We'll build the sub-directory info for the individual miners off of this.
        /// </summary>
        /// <returns></returns>
        public static string GetBaseBPMBaseDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetBaseMinersDir()
        {
            return Path.Combine(GetBaseBPMBaseDir(), "MinerApps");
        }
    }
}
