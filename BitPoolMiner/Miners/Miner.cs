using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Process;
using System;

// This is the Miner base class.

namespace BitPoolMiner.Miners
{
    public abstract class Miner
    {
        protected BPMProcess MinerProcess;

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

        protected Miner(string minerName, HardwareType hardwareType, MinerBaseType minerBaseType, bool is64Bit = true)
        {
            MinerName = minerName;
            Is64Bit = is64Bit;
            Hardware = hardwareType;
            MinerBaseType = minerBaseType;
        }

        public abstract void Start();
        public abstract void Stop();

        protected virtual BPMProcess StartProcess()
        {
            var process = new BPMProcess();

            process.Start(MinerWorkingDirectory, MinerArguments, MinerFileName);
            process.MinerProcess.Exited += MinerExited;
            return process;
        }

        protected virtual void StopProcess()
        {
            Stop();
        }

        private void MinerExited(object sender, EventArgs e)
        {
            // TODO: restart the miner if it crashed or exited.
        }

        public abstract void ReportStatsAsyc();

        protected void PostMinerMonitorStat(MinerMonitorStat stats)
        {
            // Send data to API
            MinerMonitorStatsAPI minerMonitorStatsAPI = new MinerMonitorStatsAPI();
            minerMonitorStatsAPI.PostMinerMonitorStats(stats);
        }
    }
}
