using BitPoolMiner.Models;
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
using System.Text;
using BitPoolMiner.Enums;
using System.Globalization;

namespace BitPoolMiner.Miners
{
    public class Claymore : Miner
    {
        public Claymore(HardwareType hardwareType, MinerBaseType minerBaseType) : base("Claymore", hardwareType, minerBaseType)
        {
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), "Claymore");
            MinerFileName = "EthDcrMiner64.exe";

            ApiPort = 2882;
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
            ClaymoreTemplate claymoreTemplate = await GetRPCResponse();

            if (claymoreTemplate == null)
                return;

            // Map response to BPM Statistics object
            MinerMonitorStat minerMonitorStat = new MinerMonitorStat();
            minerMonitorStat = MapRPCResponse(claymoreTemplate);

            if (minerMonitorStat == null)
                return;

            System.Threading.Thread.Sleep(2000);
            PostMinerMonitorStat(minerMonitorStat);
        }


        /// <summary>
        /// Call RPC and get response
        /// </summary>
        /// <returns></returns>
        private async Task<ClaymoreTemplate> GetRPCResponse()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Culture = CultureInfo.InvariantCulture
            };

            ClaymoreTemplate claymoreTemplate;
            try
            {
                var clientSocket = new TcpClient();
                if (clientSocket.ConnectAsync(HostName, ApiPort).Wait(5000))
                {
                    string get_menu_request = "{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}\n";
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(get_menu_request);
                    await serverStream.WriteAsync(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                    var totalBytesRead = await serverStream.ReadAsync(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream, 0, totalBytesRead);

                    claymoreTemplate = JsonConvert.DeserializeObject<ClaymoreTemplate>(returndata, jsonSettings);

                    // Close socket
                    clientSocket.Close();
                    clientSocket = null;

                    return claymoreTemplate;
                }
                else
                {
                    // TODO - Do something useful here, or ignore?
                    Console.WriteLine("Claymore socket failed");

                    // Return null object;
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO - Do something useful here
                Console.WriteLine("Claymore Exception: " + ex.Message);

                // Return null object;
                return null;
            }
        }

        /// <summary>
        /// Map Claymore response to BPM statistics objects
        /// </summary>
        /// <param name="claymoreTemplate"></param>
        /// <returns></returns>
        private MinerMonitorStat MapRPCResponse(ClaymoreTemplate claymoreTemplate)
        {
            try
            {
                // Create new Miner monitor stats object
                MinerMonitorStat minerMonitorStat = new MinerMonitorStat
                {
                    AccountGuid = (Guid)Application.Current.Properties["AccountID"],
                    WorkerName = Application.Current.Properties["WorkerName"].ToString(),
                    CoinType = this.CoinType,
                    MinerBaseType = MinerBaseType
                };

                if (claymoreTemplate.result.Count > 0)
                {
                    List<GPUMonitorStat> gpuMonitorStatList = new List<GPUMonitorStat>();
                    string[] hashRates = claymoreTemplate.result[3].Split(';');
                    string[] tempFans = claymoreTemplate.result[6].Split(';');
                    for (int i= 0; i < hashRates.GetLength(0); i++)
                    {
                        // Create new GPU monitor stats object and map values
                        GPUMonitorStat gpuMonitorStat = new GPUMonitorStat
                        {
                            AccountGuid = (Guid)Application.Current.Properties["AccountID"],
                            WorkerName = Application.Current.Properties["WorkerName"].ToString(),
                            CoinType = this.CoinType,
                            GPUID = i,
                            // Returned hashrate is in MH. Format later, return in KH/s same as CCMiner for now
                            HashRate = Convert.ToDecimal(hashRates[i])*1024,
                            FanSpeed = 0, // Let OpenHardwareMonitor get the fanspeed
                            Temp = Convert.ToInt16(tempFans[i * 2]),
                            Power = 0,
                            HardwareType = Hardware
                        };

                        // Sum up power and hashrate
                        minerMonitorStat.Power += gpuMonitorStat.Power;
                        minerMonitorStat.HashRate += gpuMonitorStat.HashRate;

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
