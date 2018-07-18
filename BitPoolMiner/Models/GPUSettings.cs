using BitPoolMiner.Enums;
using System;
using System.Collections.Generic;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Represents an individual GPU for a miner
    /// </summary>
    public class GPUSettings
    {
        /// <summary>
        /// Unique account identifier
        /// </summary>
        public Guid AccountGuid { get; set; }

        /// <summary>
        /// Name of worker
        /// </summary>
        public string WorkerName { get; set; }

        /// <summary>
        /// ID of individual GPU
        /// </summary>
        public int GPUID { get; set; }

        /// <summary>
        /// Type of card
        /// </summary>
        public HardwareType HardwareType { get; set; }

        /// <summary>
        /// Description of card
        /// </summary>
        public string HardwareName { set; get; }

        /// <summary>
        /// Is this card currently enabled for mining?
        /// </summary>
        public bool EnabledForMining { set; get; }

        /// <summary>
        /// What should this card mine?
        /// </summary>
        public CoinType CoinSelectedForMining { get; set; }

        /// <summary>
        /// What Miner should we use per card?
        /// </summary>
        public MinerBaseType MinerBaseType { get; set; }

        /// <summary>
        /// GPU Fanspeed
        /// </summary>
        public Int16 Fanspeed { get; set; }

        /// <summary>
        /// Limit Coin's to be mined based on hardware type
        /// </summary>
        public List<CoinType> CoinTypeList
        {
            get
            {
                List<CoinType> coinTypeList = new List<CoinType>();

                if (HardwareType == HardwareType.AMD)
                {
                    coinTypeList.Add(CoinType.EXP);
                }
                else if (HardwareType == HardwareType.Nvidia)
                {
                    coinTypeList.Add(CoinType.HUSH);
                    coinTypeList.Add(CoinType.KMD);
                    coinTypeList.Add(CoinType.MONA);
                    coinTypeList.Add(CoinType.VTC);
                }

                return coinTypeList;
            }
        }

        /// <summary>
        /// Limit Miner Base Type based on selected coin
        /// </summary>
        public List<MinerBaseType> MinerBaseTypeList
        {
            get
            {
                List<MinerBaseType> minerBaseTypeList = new List<MinerBaseType>();

                switch (CoinSelectedForMining)
                {
                    case CoinType.EXP:
                        minerBaseTypeList.Add(MinerBaseType.Claymore);
                        break;
                    case CoinType.HUSH:
                        minerBaseTypeList.Add(MinerBaseType.DSTM);
                        minerBaseTypeList.Add(MinerBaseType.EWBF);
                        break;
                    case CoinType.KMD:
                        minerBaseTypeList.Add(MinerBaseType.DSTM);
                        minerBaseTypeList.Add(MinerBaseType.EWBF);
                        break;
                    case CoinType.MONA:
                        minerBaseTypeList.Add(MinerBaseType.CCMiner);
                        minerBaseTypeList.Add(MinerBaseType.CCMinerNanashi);
                        break;
                    case CoinType.VTC:
                        minerBaseTypeList.Add(MinerBaseType.CCMiner);
                        minerBaseTypeList.Add(MinerBaseType.CCMinerNanashi);
                        break;
                }

                return minerBaseTypeList;
            }
        }
    }
}
