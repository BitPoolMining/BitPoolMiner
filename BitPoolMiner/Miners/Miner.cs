﻿using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Process;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

// This is the Miner base class.

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// Base miner class. Each specific miner will inherit and implement the necessary methods.
    /// </summary>
    public abstract class Miner
    {
        private const int MinerRestartDelay = 2000; // 2 second delay

        protected BPMProcess MinerProcess;
        public string Address { get; set; }
        public string MinerName { get; private set; }
        public string MinerWorkingDirectory { get; protected set; }
        public string MinerArguments { get; set; }
        public string MinerFileName { get; protected set; }
        public bool Is64Bit { get; protected set; } = true;

        // These will be used for the stats when needed
        public int ApiPort { get; protected set; }
        public string HostName { get; protected set; }

        public CoinType CoinType { get; set; }
        public HardwareType Hardware { get; protected set; }
        public MinerBaseType MinerBaseType { get; protected set; }

        public bool IsMining { get; protected set; }

        protected Miner(string minerName, HardwareType hardwareType, MinerBaseType minerBaseType, bool is64Bit = true)
        {
            MinerName = minerName;
            Is64Bit = is64Bit;
            Hardware = hardwareType;
            MinerBaseType = minerBaseType;
            IsMining = false;
        }

        /// <summary>
        /// Children must override for miner specific start operation
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// Children must override for miner specific stop operation 
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Starts the miner process
        /// </summary>
        /// <returns></returns>
        protected virtual BPMProcess StartProcess()
        {
            var process = new BPMProcess();

            IsMining = true;
            FormatPerMinerCoinCombo();

            ObservableCollection<AccountMinerTypeExtraParams> AccountMinerTypeExtraParamsList = (ObservableCollection<AccountMinerTypeExtraParams>)Application.Current.Properties["AccountMinerTypeExtraParamsList"];
            AccountMinerTypeExtraParams accountMinerTypeExtraParams = AccountMinerTypeExtraParamsList.First(x => x.MinerBaseType == MinerBaseType);

            if (accountMinerTypeExtraParams != null && accountMinerTypeExtraParams.ExtraParams != String.Empty)
            {
                // Add Extra Params 
                MinerArguments = String.Format("{0} {1}", MinerArguments, accountMinerTypeExtraParams.ExtraParams);
            }

            process.Start(MinerWorkingDirectory, MinerArguments, MinerFileName, Hardware == HardwareType.AMD, MinerBaseType);
            process.MinerProcess.Exited += MinerExited;
            return process;
        }
        /// <summary>
        /// Handle special scenarios
        /// </summary>
        private void FormatPerMinerCoinCombo()
        {
            // DS - Hotfix for Coin/Miner algo type
            // TODO - Fix this shit
            if (MinerBaseType == MinerBaseType.CryptoDredge && CoinType == CoinType.RVN)
            {
                //MinerArguments = MinerArguments.Replace("X22i", "x16r");
                MinerArguments = MinerArguments + " -c configRVN.json";
            }
            else if (MinerBaseType == MinerBaseType.CryptoDredge && CoinType == CoinType.VTC)
            {
                MinerArguments = MinerArguments + " -c configVTC.json";
            }
            else if (MinerBaseType == MinerBaseType.CCMiner && CoinType == CoinType.VTC)
            {
                MinerArguments = MinerArguments.Replace("lyra2v2", "lyra2v3");
            }
        }

        /// <summary>
        /// Stops the miner process
        /// </summary>
        protected virtual void StopProcess()
        {
            IsMining = false;
            MinerProcess?.KillProcess();
        }

        /// <summary>
        /// Handles miner exited event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MinerExited(object sender, EventArgs e)
        {
            // Restart the miner if it crashed or exited and we are still mining.
            if (IsMining)
            {
                StopProcess();
                System.Threading.Thread.Sleep(MinerRestartDelay); // a few seconds before restart
                Start();
            }
        }

        /// <summary>
        /// Reports miner statistics back to the website via API. Implemented per miner, should be asynchronous.
        /// </summary>
        public abstract void ReportStatsAsyc();

        /// <summary>
        /// Helper method to post miner stats back to the website
        /// </summary>
        /// <param name="stats"></param>
        protected void PostMinerMonitorStat(MinerMonitorStat stats)
        {
            // Use OpenHardwareMonitor to add any missing data if needed
            stats = SupplementMinerMonitorStatData(stats);

            // Send data to API
            MinerMonitorStatsAPI minerMonitorStatsAPI = new MinerMonitorStatsAPI();
            minerMonitorStatsAPI.PostMinerMonitorStats(stats);
        }

        /// <summary>
        /// Use OpenHardwareMonitor to add any missing data if needed
        /// </summary>
        /// <param name="stats"></param>
        private MinerMonitorStat SupplementMinerMonitorStatData(MinerMonitorStat stats)
        {
            // Check if any data is missing from stats
            if (CheckMinerMonitorStatDataMissing(stats) == true)
            {
                try
                {
                    // Retrive GPU data from OpenHardwareMonitor
                    Utils.OpenHardwareMonitor.OpenHardwareMonitor openHardwareMonitor = new Utils.OpenHardwareMonitor.OpenHardwareMonitor();
                    ObservableCollection<GPUSettings> gpuSettingsList = openHardwareMonitor.ScanHardware();

                    // Iterate through each GPUMonitorStat and add missing data
                    foreach (GPUMonitorStat gpuMonitorStat in stats.GPUMonitorStatList)
                    {
                        if (gpuMonitorStat.FanSpeed == 0)
                            gpuMonitorStat.FanSpeed = gpuSettingsList.Where(x => x.GPUID == gpuMonitorStat.GPUID).FirstOrDefault().Fanspeed;

                        if (gpuMonitorStat.Temp == 0)
                            gpuMonitorStat.Temp = gpuSettingsList.Where(x => x.GPUID == gpuMonitorStat.GPUID).FirstOrDefault().Temp;
                    }
                }
                catch (Exception)
                {
                    // Ignore this for now.                    
                }
            }

            return stats;
        }

        /// <summary>
        /// Check miner stats for missing data and add if needed
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        private bool CheckMinerMonitorStatDataMissing(MinerMonitorStat stats)
        {
            bool IsDataMissing = false;

            foreach (GPUMonitorStat gpuMonitorStat in stats.GPUMonitorStatList)
            {
                // Check if FanSpeed is showing 0
                if (gpuMonitorStat.FanSpeed == 0)
                    IsDataMissing = true;
            }

            return IsDataMissing;
        }
    }
}
