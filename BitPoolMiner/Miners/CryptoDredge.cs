using BitPoolMiner.Models;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;
using BitPoolMiner.Enums;
using BitPoolMiner.Utils;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for CryptoDredge derived class.
    /// </summary>
    public class CryptoDredge : Miner
    {
        public CryptoDredge(HardwareType hardwareType, MinerBaseType minerBaseType) : base("Ccminer", hardwareType, minerBaseType, true)
        {
            string versionedDirectory = "";
            MinerFileName = "CryptoDredge.exe";
            versionedDirectory = "CryptoDredge_0.16.1";

            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 4068;
            HostName = "127.0.0.1";
        }

        public override void Start()
        {
            MinerProcess = StartProcess();
        }

        public override void Stop()
        {
            try
            {
                StopProcess();
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("There was an error killing the miner process {0} with PID {1}", MinerProcess.MinerProcess.ProcessName, MinerProcess.MinerProcess.Handle), e);
            }
        }


        #region Monitoring Statistics

        public async override void ReportStatsAsyc()
        {
            try
            {
                var minerMonitorStat = await GetRPCResponse();

                if (minerMonitorStat == null)
                    return;

                System.Threading.Thread.Sleep(4000);
                PostMinerMonitorStat(minerMonitorStat);
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error reporting stats for Ccminer");
            }
        }

        private async Task<MinerMonitorStat> GetRPCResponse()
        {
            MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
            try
            {
                var dictHW = PruvotApi.GetHwInfo(HostName, ApiPort);
                var dictHist = PruvotApi.GetHistory(HostName, ApiPort);

                minerMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                minerMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                minerMonitorStat.CoinType = CoinType;
                minerMonitorStat.MinerBaseType = MinerBaseType;

                var gpuList = (from element in dictHist
                               orderby element["GPU"] ascending
                               select element["GPU"]).Distinct();

                List<GPUMonitorStat> gpuMonitorStatList = new List<GPUMonitorStat>();

                foreach (var gpuNumber in gpuList)
                {
                    var gpuHash = (from element in dictHist
                                   orderby element["GPU"] ascending, element["TS"] descending
                                   where element["GPU"] == gpuNumber
                                   select element).FirstOrDefault();

                    var gpuHw = (from hw in dictHW
                                 where hw["GPU"] == gpuNumber
                                 select hw).FirstOrDefault();

                    // Create new GPU monitor stats object and map values
                    GPUMonitorStat gpuMonitorStat = new GPUMonitorStat();

                    gpuMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                    gpuMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                    gpuMonitorStat.CoinType = CoinType.ToString();
                    gpuMonitorStat.GPUID = Convert.ToInt32(gpuNumber);
                    gpuMonitorStat.HashRate = (Convert.ToDecimal(gpuHash["KHS"]));
                    gpuMonitorStat.FanSpeed = Convert.ToInt16(gpuHw["FAN"]);
                    gpuMonitorStat.Temp = (short)Convert.ToDecimal(gpuHw["TEMP"]);
                    gpuMonitorStat.Power = (short)(Convert.ToDecimal(gpuHw["POWER"]) / 1000);
                    gpuMonitorStat.HardwareType = Hardware;

                    // Sum up power and hashrate
                    minerMonitorStat.Power += (short)gpuMonitorStat.Power;
                    minerMonitorStat.HashRate += gpuMonitorStat.HashRate;

                    // Add GPU stats to list
                    gpuMonitorStatList.Add(gpuMonitorStat);

                }

                // Set list of GPU monitor stats
                minerMonitorStat.GPUMonitorStatList = gpuMonitorStatList;

                return await Task.Run(() => { return minerMonitorStat; });

            }
            catch (Exception ex)
            {
                NLogProcessing.LogError(ex, "Error calling GetRPCResponse from miner.");

                // Return null object;
                return null;
            }
        }

        #endregion
    }   
}