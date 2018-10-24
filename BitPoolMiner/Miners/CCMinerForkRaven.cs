using System.IO;
using BitPoolMiner.Enums;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for ccminer RavenCoin fork derived class.
    /// </summary>
    public class CCMinerForkRaven : Ccminer
    {
        public CCMinerForkRaven(HardwareType hardwareType, MinerBaseType minerBaseType) : base(hardwareType, minerBaseType, false)
        {
            string versionedDirectory = "";
            MinerFileName = "ccminer.exe";
            versionedDirectory = "ccminer-ravencoin-v2.6";
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 4093;
            HostName = "127.0.0.1";
        }

    }
}
