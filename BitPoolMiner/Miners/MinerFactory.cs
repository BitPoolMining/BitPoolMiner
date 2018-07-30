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

                case MinerBaseType.CCMinerNanashi:
                    return new CCMinerForkNanashi(hardwareType, minerBaseType);

                case MinerBaseType.EWBF:
                    return new EWBF(hardwareType, minerBaseType);

                case MinerBaseType.EWBF_NO_ASIC:
                    return new EWBF_NO_ASIC(hardwareType, minerBaseType);

                case MinerBaseType.DSTM:
                    return new DSTM(hardwareType, minerBaseType);

                case MinerBaseType.Claymore:
                    return new Claymore(hardwareType, minerBaseType);

                default:
                    throw new ApplicationException(string.Format("The miner base type {0} is not yet supported.", minerBaseType.ToString()));
            }
        }
    }
}
