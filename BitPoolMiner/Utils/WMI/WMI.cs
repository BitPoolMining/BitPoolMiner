using BitPoolMiner.Models;
using System;
using System.Collections.ObjectModel;
using System.Management;
using System.Windows;

namespace BitPoolMiner.Utils.WMI
{
    class WMI
    {
        public ObservableCollection<GPUSettings> ScanHardware()
        {
            ObservableCollection<GPUSettings> gpuSettingsList = new ObservableCollection<GPUSettings>();

            try
            {
                ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");

                foreach (ManagementObject obj in objvide.Get())
                {
                    GPUSettings gpuSettings = new GPUSettings();

                    gpuSettings.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                    gpuSettings.WorkerName = Application.Current.Properties["WorkerName"].ToString();
                    gpuSettings.HardwareName = obj["Name"].ToString();
                    gpuSettings.GPUID = Int32.Parse(obj["DeviceID"].ToString());
                    gpuSettings.EnabledForMining = true;

                    gpuSettingsList.Add(gpuSettings);
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

