using BitPoolMiner.Models;
using BitPoolMiner.Models.RPCResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using BitPoolMiner.Enums;
using BitPoolMiner.Utils;
using System.Collections.Specialized;
using System.Net;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for ccminer WildRig fork derived class.
    /// </summary>
    public class WildRig : Miner
    {
        public WildRig(HardwareType hardwareType, MinerBaseType minerBaseType) : base("WildRig", hardwareType, minerBaseType)
        {
            string versionedDirectory = "";
            MinerFileName = "wildrig.exe";
            versionedDirectory = "wildrig-multi-0.13.4-beta";
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 2883;
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

        /// <summary>
        /// Collect stats from WildRig and post to API
        /// </summary>
        public override async void ReportStatsAsyc()
        {
            try
            {
                // Call RPC and get response
                WildRigTemplate wildRigTemplate = await GetRPCResponse();

                if (wildRigTemplate == null)
                    return;

                // Map response to BPM Statistics object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat = MapRPCResponse(wildRigTemplate);

                if (minerMonitorStat == null)
                    return;

                System.Threading.Thread.Sleep(8000);
                PostMinerMonitorStat(minerMonitorStat);
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error reporting stats for WildRig");
            }
        }

        /// <summary>
        /// Call RPC and get response
        /// </summary>
        /// <returns></returns>
        private async Task<WildRigTemplate> GetRPCResponse()
        {
            try
            {
                string apiURL = String.Format("http://{0}:{1}", HostName, ApiPort);
                WildRigTemplate wildRigTemplate = DownloadSerializedJSONData<WildRigTemplate>(apiURL);

                return wildRigTemplate;
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, $"Error reading RPC call from WildRig miner on port {ApiPort}");

                // Return null object;
                return null;
            }
        }

        /// <summary>
        /// Retrieve data from API, deserialize and parse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        private T DownloadSerializedJSONData<T>(string url) where T : new()
        {
            using (var webClient = new WebClient())
            {
                var jsonData = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    jsonData = webClient.DownloadString(url);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Error call API at {0}", url), e);
                }

                // If string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<T>(jsonData) : new T();
            }
        }

        /// <summary>
        /// Map WildRig response to BPM statistics objects
        /// </summary>
        /// <param name="wildRigTemplate"></param>
        /// <returns></returns>
        private MinerMonitorStat MapRPCResponse(WildRigTemplate wildRigTemplate)
        {
            try
            {
                // Used to simulate GPU ID
                int gpuCounter = 0;

                // Create new Miner monitor stats object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                minerMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                minerMonitorStat.CoinType = this.CoinType;
                minerMonitorStat.MinerBaseType = MinerBaseType;


                if (wildRigTemplate.hashrate.threads.Count > 0)
                {
                    List<GPUMonitorStat> gpuMonitorStatList = new List<GPUMonitorStat>();

                    foreach (List<int> wildRigGPU in wildRigTemplate.hashrate.threads)
                    {
                        // Create new GPU monitor stats object and map values
                        GPUMonitorStat gpuMonitorStat = new GPUMonitorStat
                        {
                            AccountGuid = (Guid)Application.Current.Properties["AccountID"],
                            WorkerName = Application.Current.Properties["WorkerName"].ToString(),
                            CoinType = this.CoinType.ToString(),
                            GPUID = gpuCounter,//wildRigGPU.device_id,
                            HashRate = wildRigGPU[0],
                            FanSpeed = 0,//(short)wildRigGPU.fan_speed,
                            Temp = 0,//(short)wildRigGPU.temperature,
                            Power = 0,//(short)wildRigGPU.power,
                            HardwareType = Hardware
                        };

                        // Sum up power and hashrate
                        minerMonitorStat.Power += 0; //wildRigGPU.power;
                        minerMonitorStat.HashRate += wildRigGPU[0];

                        // Add GPU stats to list
                        gpuMonitorStatList.Add(gpuMonitorStat);

                        // Increase GPU Counter
                        gpuCounter++;
                    }

                    // Set list of GPU monitor stats
                    minerMonitorStat.GPUMonitorStatList = gpuMonitorStatList;
                }

                return minerMonitorStat;
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error mapping RPC Response for WildRig miner");
                return null;
            }
        }

        #endregion

    }
}
