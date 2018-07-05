using BitPoolMiner.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Miners
{
    public class MinerFactory
    {
        public static Miner CreateMiner(MinerBaseType minerBaseType, HardwareType hardwareType, bool is64Bit = true)
        {
            switch (minerBaseType)
            {
                case MinerBaseType.CCMiner:
                    return new Ccminer(hardwareType, minerBaseType, is64Bit);

                case MinerBaseType.EWBF:
                    return new EWBF(hardwareType, minerBaseType);

                case MinerBaseType.DSTM:
                    return new DSTM(hardwareType, minerBaseType);

                default:
                    throw new ApplicationException(string.Format("The miner base type {0} is not yet supported.", minerBaseType.ToString()));
            }
        }
    }
}
