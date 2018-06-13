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
using System.Globalization;
using System.Linq;
using BitPoolMiner.Enums;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for ccminer override.
    /// </summary>
    public class Ccminer : Miner
    {
        public Ccminer(HardwareType hardwareType, MinerBaseType minerBaseType, bool is64Bit) : base("Ccminer", hardwareType, minerBaseType, is64Bit)
        {
            string versionedDirectory = "";
            if (is64Bit)
            {
                MinerFileName = "ccminer-x64.exe";
                versionedDirectory = "ccminer-x64-2.2.4-cuda9";
            }
            else
            {
                MinerFileName = "ccminer.exe";
                versionedDirectory = "ccminer-x86-2.2.4-cuda9";
            }
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
            var minerMonitorStat = await GetRPCResponse();

            if (minerMonitorStat == null)
                return;

            PostMinerMonitorStat(minerMonitorStat);
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
                    gpuMonitorStat.CoinType = CoinType;
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
                // TODO - Do something useful here
                Console.WriteLine("ccminer Exception: " + ex.Message);

                // Return null object;
                return null;
            }
        }


    }

    internal static class PruvotApi
    {
        // Converts the results from Pruvots api to an array of dictionaries to get more JSON like results
        public static Dictionary<string, string>[] ConvertPruvotToDictArray(string apiResult)
        {
            if (apiResult == null) return null;

            // Will return a Dict per GPU
            string[] splitGroups = apiResult.Split('|');

            Dictionary<string, string>[] totalDict = new Dictionary<string, string>[splitGroups.Length - 1];

            for (int index = 0; index < splitGroups.Length - 1; index++)
            {
                Dictionary<string, string> separateDict = new Dictionary<string, string>();
                string[] keyValues = splitGroups[index].Split(';');
                for (int i = 0; i < keyValues.Length; i++)
                {
                    string[] elements = keyValues[i].Split('=');
                    if (elements.Length > 1) separateDict.Add(elements[0], elements[1]);
                }
                totalDict[index] = separateDict;
            }

            return totalDict;
        }

        public static T GetDictValue<T>(Dictionary<string, string> dictionary, string key)
        {
            string value;

            if (dictionary.TryGetValue(key, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            // Unsigneds can't be negative
            if (typeof(T) == typeof(uint)) return (T)Convert.ChangeType(9001, typeof(T), CultureInfo.InvariantCulture);
            return (T)Convert.ChangeType(-1, typeof(T), CultureInfo.InvariantCulture);
        }

        // Overload that just returns the string without type conversion
        public static string GetDictValue(Dictionary<string, string> dictionary, string key)
        {
            string value;

            if (dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return "-1";
        }

        public static Dictionary<string, string>[] GetSummary(string ip = "127.0.0.1", int port = 4068)
        {
            return Request(ip, port, "summary");
        }

        public static Dictionary<string, string>[] GetHwInfo(string ip = "127.0.0.1", int port = 4068)
        {
            return Request(ip, port, "hwinfo");
        }

        public static Dictionary<string, string>[] GetMemInfo(string ip = "127.0.0.1", int port = 4068)
        {
            return Request(ip, port, "meminfo");
        }

        public static Dictionary<string, string>[] GetThreads(string ip = "127.0.0.1", int port = 4068)
        {
            return Request(ip, port, "threads");
        }

        public static Dictionary<string, string>[] GetHistory(string ip = "127.0.0.1", int port = 4068, int minerMap = -1)
        {
            Dictionary<string, string>[] histo = Request(ip, port, "histo");

            if (histo == null) return null;

            bool existsInHisto = false;
            foreach (Dictionary<string, string> log in histo)
            {
                if (GetDictValue<int>(log, "GPU") == minerMap)
                {
                    existsInHisto = true;
                    break;
                }
            }

            if (existsInHisto || minerMap == -1) return minerMap == -1 ? histo : Request(ip, port, "histo|" + minerMap);

            return null;
        }

        public static Dictionary<string, string>[] GetPoolInfo(string ip = "127.0.0.1", int port = 4068)
        {
            return Request(ip, port, "pool");
        }

        public static Dictionary<string, string>[] GetScanLog(string ip = "127.0.0.1", int port = 4068)
        {
            return Request(ip, port, "scanlog");
        }

        public static Dictionary<string, string>[] Request(string ip, int port, string message)
        {
            string responseData = "";

            try
            {
                using (TcpClient client = new TcpClient(ip, port))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                        stream.Flush();

                        data = new Byte[4096];

                        int bytes = stream.Read(data, 0, data.Length);
                        responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    }

                }
            }
            catch (Exception)
            {
                return null;
            }


            return ConvertPruvotToDictArray(responseData);
        }
    }

    #endregion
}
