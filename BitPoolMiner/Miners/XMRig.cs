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
    public class XMRig : Miner
    {
        public XMRig(HardwareType hardwareType, MinerBaseType minerBaseType) : base("XMRig", hardwareType, minerBaseType)
        {
            string versionedDirectory = "";
            MinerFileName = "xmrig-notls.exe";
            versionedDirectory = "xmrig-2.14.4";
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 8080;
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
        /// Collect stats from XMRig and post to API
        /// </summary>
        public override async void ReportStatsAsyc()
        {
            try
            {
                // Call RPC and get response
                XMRigTemplate xmrigTemplate = await GetRPCResponse();

                if (xmrigTemplate == null)
                    return;

                // Map response to BPM Statistics object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat = MapRPCResponse(xmrigTemplate);

                if (minerMonitorStat == null)
                    return;

                System.Threading.Thread.Sleep(8000);
                PostMinerMonitorStat(minerMonitorStat);
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error reporting stats for XMRig");
            }
        }

        /// <summary>
        /// Call RPC and get response
        /// </summary>
        /// <returns></returns>
        private async Task<XMRigTemplate> GetRPCResponse()
        {
            try
            {
                string apiURL = String.Format("http://{0}:{1}", HostName, ApiPort);
                XMRigTemplate xmrigTemplate = DownloadSerializedJSONData<XMRigTemplate>(apiURL);

                return xmrigTemplate;
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, $"Error reading RPC call from XMRig miner on port {ApiPort}");

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
        /// Map XMRig response to BPM statistics objects
        /// </summary>
        /// <param name="XMRigTemplate"></param>
        /// <returns></returns>
        private MinerMonitorStat MapRPCResponse(XMRigTemplate xmrigTemplate)
        {
            try
            {
                // Create new Miner monitor stats object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                minerMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                minerMonitorStat.CoinType = this.CoinType;
                minerMonitorStat.MinerBaseType = MinerBaseType;

               
                List<GPUMonitorStat> gpuMonitorStatsList = new List<GPUMonitorStat>();

                GPUMonitorStat gpuMonitorStat = new GPUMonitorStat();
                gpuMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                gpuMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                gpuMonitorStat.CoinType = this.CoinType.ToString();
                gpuMonitorStat.GPUID = 0;
                gpuMonitorStat.HardwareName = xmrigTemplate.cpu.brand;
                gpuMonitorStat.HashRate = (decimal)xmrigTemplate.hashrate.highest;
                gpuMonitorStat.FanSpeed = 0;
                gpuMonitorStat.Temp = 0;
                gpuMonitorStat.Power = 0;
                gpuMonitorStat.HardwareType = Hardware;

                // Sum up power and hashrate
                minerMonitorStat.Power += 0;
                minerMonitorStat.HashRate += gpuMonitorStat.HashRate;

                // Add GPU stats to list
                gpuMonitorStatsList.Add(gpuMonitorStat);
                    

                // Set list of GPU monitor stats
                minerMonitorStat.GPUMonitorStatList = gpuMonitorStatsList;


                return minerMonitorStat;
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error mapping RPC Response for XMRig miner");
                return null;
            }
        }

        #endregion

    }
}