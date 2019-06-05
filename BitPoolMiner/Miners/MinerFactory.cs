using BitPoolMiner.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPoolMiner.Utils;

namespace BitPoolMiner.Miners
{
    /// <summary>
    /// This is the miner factor to construct each miner for each mining session instance
    /// </summary>
    public class MinerFactory
    {
        public static Miner CreateMiner(MinerBaseType minerBaseType, HardwareType hardwareType, bool is64Bit = true)
        {
            switch (minerBaseType)
            {
                case MinerBaseType.CCMiner:
                    return new Ccminer(hardwareType, minerBaseType, is64Bit);

                case MinerBaseType.Claymore:
                    return new Claymore(hardwareType, minerBaseType);

                case MinerBaseType.TRex:
                    return new TRex(hardwareType, minerBaseType);

                case MinerBaseType.WildRig:
                    return new WildRig(hardwareType, minerBaseType);

                case MinerBaseType.CryptoDredge:
                    return new CryptoDredge(hardwareType, minerBaseType);

                case MinerBaseType.LyclMiner:
                    return new LyclMiner(hardwareType, minerBaseType, is64Bit);

                default:
                    throw new ApplicationException(string.Format("The miner base type {0} is not yet supported.", minerBaseType.ToString()));
            }
        }
    }
}
