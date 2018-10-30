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
    /// This class is for ccminer TRex fork derived class.
    /// </summary>
    public class TRex : Miner
    {
        public TRex(HardwareType hardwareType, MinerBaseType minerBaseType) : base("TRex", hardwareType, minerBaseType)
        {
            string versionedDirectory = "";
            MinerFileName = "t-rex.exe";
            versionedDirectory = "t-rex-0.7.2-win-cuda10.0";
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 4067;
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
        /// Collect stats from TRex and post to API
        /// </summary>
        public override async void ReportStatsAsyc()
        {
            try
            {
                // Call RPC and get response
                TRexTemplate trexTemplate = await GetRPCResponse();

                if (trexTemplate == null)
                    return;

                // Map response to BPM Statistics object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat = MapRPCResponse(trexTemplate);

                if (minerMonitorStat == null)
                    return;

                System.Threading.Thread.Sleep(8000);
                PostMinerMonitorStat(minerMonitorStat);
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error reporting stats for TRex");
            }
        }

        /// <summary>
        /// Call RPC and get response
        /// </summary>
        /// <returns></returns>
        private async Task<TRexTemplate> GetRPCResponse()
        {
            try
            {
                // Build WhatToMine API URL from Coin ID Dictionary to get expected coin ID
                string apiURL = String.Format("http://{0}:{1}/summary", HostName, ApiPort);
                TRexTemplate trexTemplate = DownloadSerializedJSONData<TRexTemplate>(apiURL);

                return trexTemplate;
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, $"Error reading RPC call from TRex miner on port {ApiPort}");

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
        /// Map TRex response to BPM statistics objects
        /// </summary>
        /// <param name="trexTemplate"></param>
        /// <returns></returns>
        private MinerMonitorStat MapRPCResponse(TRexTemplate trexTemplate)
        {
            try
            {
                // Create new Miner monitor stats object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                minerMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                minerMonitorStat.CoinType = this.CoinType;
                minerMonitorStat.MinerBaseType = MinerBaseType;

                if (trexTemplate.gpus.Count > 0)
                {
                    List<GPUMonitorStat> gpuMonitorStatList = new List<GPUMonitorStat>();

                    foreach (GPUList trexGPU in trexTemplate.gpus)
                    {
                        // Create new GPU monitor stats object and map values
                        GPUMonitorStat gpuMonitorStat = new GPUMonitorStat
                        {
                            AccountGuid = (Guid)Application.Current.Properties["AccountID"],
                            WorkerName = Application.Current.Properties["WorkerName"].ToString(),
                            CoinType = this.CoinType,
                            GPUID = trexGPU.device_id,
                            HashRate = trexGPU.hashrate,
                            FanSpeed = (short)trexGPU.fan_speed,
                            Temp = 0,
                            Power = 0,
                            HardwareType = Hardware
                        };

                        // Sum up power and hashrate
                        minerMonitorStat.Power += 0;
                        minerMonitorStat.HashRate += trexGPU.hashrate;

                        // Add GPU stats to list
                        gpuMonitorStatList.Add(gpuMonitorStat);
                    }

                    // Set list of GPU monitor stats
                    minerMonitorStat.GPUMonitorStatList = gpuMonitorStatList;
                }

                return minerMonitorStat;
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error mapping RPC Response for TRex miner");
                return null;
            }
        }

        #endregion

    }
}
