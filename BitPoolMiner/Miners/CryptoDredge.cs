using System.IO;
using BitPoolMiner.Enums;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This class is for CryptoDredge which is ccminer fork derived class.
    /// </summary>
    public class CryptoDredge : Ccminer
    {
        public CryptoDredge(HardwareType hardwareType, MinerBaseType minerBaseType) : base(hardwareType, minerBaseType, false)
        {
            string versionedDirectory = "";
            MinerFileName = "CryptoDredge.exe";
            versionedDirectory = "CryptoDredge_0.11.0_win_x64";
            MinerWorkingDirectory = Path.Combine(Utils.Core.GetBaseMinersDir(), versionedDirectory);

            ApiPort = 4068;
            HostName = "127.0.0.1";
        }

    }
}

