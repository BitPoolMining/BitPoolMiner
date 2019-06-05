using BitPoolMiner.Models;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Hardware.ATI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BitPoolMiner.Utils.OpenHardwareMonitor
{
    class OpenHardwareMonitor
    {
        public ObservableCollection<GPUSettings> ScanHardware()
        {
            ObservableCollection<GPUSettings> gpuSettingsList = new ObservableCollection<GPUSettings>();

            try
            {
                Computer myComputer = new Computer();
                myComputer.Open();
                myComputer.GPUEnabled = true;

                int nvidiaCount = 0;
                int amdCount = 0; 

                foreach (var hardwareItem in myComputer.Hardware)
                {
                    if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti)
                    {
                        GPUSettings gpuSettings = new GPUSettings();

                        gpuSettings.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                        gpuSettings.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                        gpuSettings.HardwareName = hardwareItem.Name;
                        gpuSettings.EnabledForMining = true;
                        
                        try
                        {
                            gpuSettings.Fanspeed = Convert.ToInt16(hardwareItem.Sensors.Where(x => x.SensorType == SensorType.Control && x.Name == "GPU Fan").FirstOrDefault().Value);
                        }
                        catch
                        {
                            gpuSettings.Fanspeed = 0;
                        }

                        try
                        {
                            gpuSettings.Temp = Convert.ToInt16(hardwareItem.Sensors.Where(x => x.SensorType == SensorType.Temperature).FirstOrDefault().Value);
                        }
                        catch
                        {
                            gpuSettings.Temp = 0;
                        }

                        gpuSettings.EnabledForMining = true;

                        if (hardwareItem.HardwareType == HardwareType.GpuNvidia)
                        {
                            //gpuSettings.GPUID = Convert.ToUInt16(hardwareItem.Identifier.ToString().Replace("/nvidiagpu/", "").Replace("/atigpu/", "").Replace("}", ""));

                            gpuSettings.GPUID = nvidiaCount;
                            nvidiaCount++;

                            gpuSettings.HardwareType = Enums.HardwareType.Nvidia;
                            gpuSettings.CoinSelectedForMining = Enums.CoinType.RVN;
                            gpuSettings.MinerBaseType = Enums.MinerBaseType.CryptoDredge;

                        }
                        else if (hardwareItem.HardwareType == HardwareType.GpuAti)
                        {
                            //gpuSettings.GPUID = Convert.ToUInt16(hardwareItem.Identifier.ToString().Replace("/nvidiagpu/", "").Replace("/atigpu/", "").Replace("}", ""));

                            gpuSettings.GPUID = amdCount;
                            amdCount++;

                            gpuSettings.HardwareType = Enums.HardwareType.AMD;
                            gpuSettings.CoinSelectedForMining = Enums.CoinType.ETC;
                            gpuSettings.MinerBaseType = Enums.MinerBaseType.Claymore;
                        }

                        // Add GPU settings to list
                        gpuSettingsList.Add(gpuSettings);
                    }
                }

                return gpuSettingsList;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error scanning hardware"), e);
            }
        }
    }
}
