using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace BitPoolMiner.Formatter
{
    public static class MinerMonitorStatsFormatter
    {
        public static ObservableCollection<MinerMonitorStat> FormatMinerMonitorStats(ObservableCollection<MinerMonitorStat> minerMonitorStatList)
        {
            try
            {
                foreach (MinerMonitorStat minerMonitorStat in minerMonitorStatList)
                {
                    // Update coin logo for each miner
                    CoinLogos.CoinLogoDictionary.TryGetValue(minerMonitorStat.CoinType, out string logoSourceLocation);
                    if (minerMonitorStat.CoinType != CoinType.UNDEFINED)
                        minerMonitorStat.CoinLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logoSourceLocation);
                }

                return minerMonitorStatList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error loading monitor stats", ex);
            }
        }

    }
}
