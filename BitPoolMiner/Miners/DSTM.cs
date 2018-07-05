﻿using BitPoolMiner.Models;
using BitPoolMiner.Models.RPCResponse;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Process;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using BitPoolMiner.Enums;

namespace BitPoolMiner.Miners
{
    public class DSTM : Miner
    {
        public DSTM(HardwareType hardwareType, MinerBaseType minerBaseType) : base("DSTM", hardwareType, minerBaseType)
        {
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), "DSTM");
            MinerFileName = "zm.exe";

            ApiPort = 2222;
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
        /// Collect stats from EWBF and post to API
        /// </summary>
        public override async void ReportStatsAsyc()
        {
            // Call RPC and get response
            DSTMTemplate dstmTemplate = await GetRPCResponse();

            if (dstmTemplate == null)
                return;

            // Map response to BPM Statistics object
            MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
            minerMonitorStat = MapRPCResponse(dstmTemplate);

            if (minerMonitorStat == null)
                return;

            PostMinerMonitorStat(minerMonitorStat);
        }

        /// <summary>
        /// Call RPC and get response
        /// </summary>
        /// <returns></returns>
        private async Task<DSTMTemplate> GetRPCResponse()
        {
            DSTMTemplate dstmTemplate;
            try
            {
                var clientSocket = new TcpClient();

                if (clientSocket.ConnectAsync(HostName, ApiPort).Wait(5000))
                {
                    string get_menu_request = "{\"id\":1, \"method\":\"getstat\"}\n";
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(get_menu_request);
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                    await serverStream.ReadAsync(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    string _returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    string jsonData = _returndata.Substring(0, _returndata.LastIndexOf("}") + 1);

                    dstmTemplate = JsonConvert.DeserializeObject<DSTMTemplate>(jsonData);

                    // Close socket
                    clientSocket.Close();
                    clientSocket = null;

                    return dstmTemplate;
                }
                else
                {
                    // TODO - Do something useful here, or ignore?
                    Console.WriteLine("DSTM socket failed");

                    // Return null object;
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO - Do something useful here
                Console.WriteLine("DSTM Exception: " + ex.Message);

                // Return null object;
                return null;
            }
        }

        /// <summary>
        /// Map DSTM response to BPM statistics objects
        /// </summary>
        /// <param name="dstmTemplate"></param>
        /// <returns></returns>
        private MinerMonitorStat MapRPCResponse(DSTMTemplate dstmTemplate)
        {
            try
            {
                // Create new Miner monitor stats object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                minerMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                minerMonitorStat.CoinType = this.CoinType;
                minerMonitorStat.MinerBaseType = MinerBaseType;

                if (dstmTemplate.result.Count > 0)
                {
                    List<GPUMonitorStat> gpuMonitorStatList = new List<GPUMonitorStat>();

                    foreach (DSTMOBjectTemplate dstmOBjectTemplate in dstmTemplate.result)
                    {
                        // Create new GPU monitor stats object and map values
                        GPUMonitorStat gpuMonitorStat = new GPUMonitorStat
                        {
                            AccountGuid = (Guid)Application.Current.Properties["AccountID"],
                            WorkerName = Application.Current.Properties["WorkerName"].ToString(),
                            CoinType = this.CoinType,
                            GPUID = dstmOBjectTemplate.gpu_id,
                            HashRate = dstmOBjectTemplate.sol_ps,
                            FanSpeed = 0,
                            Temp = (short)dstmOBjectTemplate.temperature,
                            Power = (short)dstmOBjectTemplate.power_usage,
                            HardwareType = Hardware
                        };

                        // Sum up power and hashrate
                        minerMonitorStat.Power += (short)dstmOBjectTemplate.power_usage;
                        minerMonitorStat.HashRate += dstmOBjectTemplate.sol_ps;

                        // Add GPU stats to list
                        gpuMonitorStatList.Add(gpuMonitorStat);
                    }

                    // Set list of GPU monitor stats
                    minerMonitorStat.GPUMonitorStatList = gpuMonitorStatList;
                }

                return minerMonitorStat;
            }
            catch (Exception)
            {
                // Todo - Add error handling and do something useful here
                return null;
            }
        }

        #endregion

    }
}