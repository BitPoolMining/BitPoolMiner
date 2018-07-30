using System.IO;
using BitPoolMiner.Enums;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for ccminer Nanashi fork derived class.
    /// </summary>
    public class CCMinerForkNanashi : Ccminer
    {
        public CCMinerForkNanashi(HardwareType hardwareType, MinerBaseType minerBaseType) : base(hardwareType, minerBaseType, false)
        {
            string versionedDirectory = "";
            MinerFileName = "ccminer.exe";
            versionedDirectory = "ccminer-2.2-mod-r2";
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 4093;
            HostName = "127.0.0.1";
        }

    }
}
