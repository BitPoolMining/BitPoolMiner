using BitPoolMiner.Models;
using OpenHardwareMonitor.Hardware;
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

                foreach (var hardwareItem in myComputer.Hardware)
                {
                    if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAti)
                    {
                        GPUSettings gpuSettings = new GPUSettings();

                        gpuSettings.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                        gpuSettings.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                        gpuSettings.GPUID = Convert.ToUInt16(hardwareItem.Identifier.ToString().Replace("/nvidiagpu/","").Replace("/atigpu/", "").Replace("}", ""));
                        gpuSettings.HardwareName = hardwareItem.Name;
                        gpuSettings.EnabledForMining = true;
                        gpuSettings.Fanspeed = Convert.ToInt16(hardwareItem.Sensors.Where(x => x.SensorType == SensorType.Control && x.Name == "GPU Fan").FirstOrDefault().Value);
                        gpuSettings.EnabledForMining = true;

                        if (hardwareItem.HardwareType == HardwareType.GpuNvidia)
                        {
                            gpuSettings.HardwareType = Enums.HardwareType.Nvidia;
                            gpuSettings.CoinSelectedForMining = Enums.CoinType.HUSH;
                            gpuSettings.MinerBaseType = Enums.MinerBaseType.EWBF;

                        }
                        else if (hardwareItem.HardwareType == HardwareType.GpuAti)
                        {
                            gpuSettings.HardwareType = Enums.HardwareType.AMD;
                            gpuSettings.CoinSelectedForMining = Enums.CoinType.EXP;
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
