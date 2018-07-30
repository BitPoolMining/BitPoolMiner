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

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for EWBF miner derived class.
    /// </summary>
    public class EWBF : Miner
    {

        public EWBF(HardwareType hardwareType, MinerBaseType minerBaseType) : base("EWBF", hardwareType, minerBaseType)
        {
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), "EWBF");
            MinerFileName = "miner.exe";

            ApiPort = 42000;
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
            try
            {
                // Call RPC and get response
                EWBFTemplate ewbfTemplate = await GetRPCResponse();

                if (ewbfTemplate == null)
                    return;

                // Map response to BPM Statistics object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat = MapRPCResponse(ewbfTemplate);

                if (minerMonitorStat == null)
                    return;

                System.Threading.Thread.Sleep(8000);
                PostMinerMonitorStat(minerMonitorStat);
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, "Error reporting stats for EWBF");
            }
        }

        /// <summary>
        /// Call RPC and get response
        /// </summary>
        /// <returns></returns>
        private async Task<EWBFTemplate> GetRPCResponse()
        {
            EWBFTemplate ewbfTemplate;
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

                    ewbfTemplate = JsonConvert.DeserializeObject<EWBFTemplate>(jsonData);

                    // Close socket
                    clientSocket.Close();
                    clientSocket = null;

                    return ewbfTemplate;
                }
                else
                {
                    NLogProcessing.LogInfo($"Could not connect to EWBF miner socket on port {ApiPort}");

                    // Return null object;
                    return null;
                }
            }
            catch (Exception e)
            {
                NLogProcessing.LogError(e, $"Error reading RPC call from EWBF miner on port {ApiPort}");

                // Return null object;
                return null;
            }
        }

        /// <summary>
        /// Map EWBF response to BPM statistics objects
        /// </summary>
        /// <param name="ewbfTemplate"></param>
        /// <returns></returns>
        private MinerMonitorStat MapRPCResponse(EWBFTemplate ewbfTemplate)
        {
            try
            {
                // Create new Miner monitor stats object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
                minerMonitorStat.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                minerMonitorStat.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                minerMonitorStat.CoinType = this.CoinType;
                minerMonitorStat.MinerBaseType = MinerBaseType;

                if (ewbfTemplate.result.Count > 0)
                {
                    List<GPUMonitorStat> gpuMonitorStatList = new List<GPUMonitorStat>();

                    foreach (EWBFOBjectTemplate ewbfOBjectTemplate in ewbfTemplate.result)
                    {
                        // Create new GPU monitor stats object and map values
                        GPUMonitorStat gpuMonitorStat = new GPUMonitorStat
                        {
                            AccountGuid = (Guid)Application.Current.Properties["AccountID"],
                            WorkerName = Application.Current.Properties["WorkerName"].ToString(),
                            CoinType = this.CoinType,
                            GPUID = ewbfOBjectTemplate.gpuid,
                            HashRate = ewbfOBjectTemplate.speed_sps,
                            FanSpeed = 0,
                            Temp = (short)ewbfOBjectTemplate.temperature,
                            Power = (short)ewbfOBjectTemplate.gpu_power_usage,
                            HardwareType = Hardware
                        };

                        // Sum up power and hashrate
                        minerMonitorStat.Power += (short)ewbfOBjectTemplate.gpu_power_usage;
                        minerMonitorStat.HashRate += ewbfOBjectTemplate.speed_sps;

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
                NLogProcessing.LogError(e, "Error mapping RPC Response for EWBF miner");

                return null;
            }
        }

        #endregion
    }
}
