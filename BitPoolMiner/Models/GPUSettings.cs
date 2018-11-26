using BitPoolMiner.Enums;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;

namespace BitPoolMiner.Models
{
    /// <summary>
    /// Represents an individual GPU for a miner
    /// </summary>
    public class GPUSettings : ViewModelBase
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
        private CoinType coinSelectedForMining;
        public CoinType CoinSelectedForMining
        {
            get
            {
                return coinSelectedForMining;
            }
            set
            {
                coinSelectedForMining = value;
                minerBaseTypeList = SetMinerBaseType();

                if (minerBaseTypeList.Count > 0)
                {
                    MinerBaseType = minerBaseTypeList[0];
                    OnPropertyChanged("CoinSelectedForMining");
                    OnPropertyChanged("MinerBaseTypeList");
                    OnPropertyChanged("MinerBaseType");
                }
            }
        }

        /// <summary>
        /// What Miner should we use per card?
        /// </summary>
        public MinerBaseType MinerBaseType { get; set; }

        /// <summary>
        /// GPU Fanspeed
        /// </summary>
        public Int16 Fanspeed { get; set; }

        /// <summary>
        /// GPU Temp
        /// </summary>
        public Int16 Temp { get; set; }

        /// <summary>
        /// Limit Coin's to be mined based on hardware type
        /// </summary>
        public List<CoinType> CoinTypeList
        {
            get
            {
                return SetCoinList();
            }
            set
            {
                minerBaseTypeList = SetMinerBaseType();
                OnPropertyChanged();
            }

        }
        private List<CoinType> SetCoinList()
        {
            List<CoinType> coinTypeList = new List<CoinType>();

            if (HardwareType == HardwareType.AMD)
            {
                coinTypeList.Add(CoinType.EXP);
                coinTypeList.Add(CoinType.ETH);
                coinTypeList.Add(CoinType.ETC);
                coinTypeList.Add(CoinType.RVN);
            }
            else if (HardwareType == HardwareType.Nvidia)
            {
                coinTypeList.Add(CoinType.HUSH);
                coinTypeList.Add(CoinType.KMD);
                coinTypeList.Add(CoinType.MONA);
                coinTypeList.Add(CoinType.VTC);
                coinTypeList.Add(CoinType.ZCL);
                coinTypeList.Add(CoinType.ZEN);
                coinTypeList.Add(CoinType.BTG);
                coinTypeList.Add(CoinType.BTCP);
                coinTypeList.Add(CoinType.RVN);
                coinTypeList.Add(CoinType.SUQA);
            }

            return coinTypeList;
        }

        /// <summary>
        /// Limit Miner Base Type based on selected coin
        /// </summary>
        private List<MinerBaseType> minerBaseTypeList;
        public List<MinerBaseType> MinerBaseTypeList
        {
            get
            {
                if (minerBaseTypeList == null || minerBaseTypeList.Count == 0)
                {
                    minerBaseTypeList = SetMinerBaseType();
                }
                return minerBaseTypeList;
            }
        }
        private List<MinerBaseType> SetMinerBaseType()
        {
            List<MinerBaseType> minerBaseTypeList = new List<MinerBaseType>();

            switch (CoinSelectedForMining)
            {
                case CoinType.EXP:
                    minerBaseTypeList.Add(MinerBaseType.Claymore);
                    break;
                case CoinType.ETH:
                    minerBaseTypeList.Add(MinerBaseType.Claymore);
                    break;
                case CoinType.ETC:
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
                case CoinType.BTG:
                    minerBaseTypeList.Add(MinerBaseType.EWBF_NO_ASIC);
                    break;
                case CoinType.BTCP:
                    minerBaseTypeList.Add(MinerBaseType.DSTM);
                    minerBaseTypeList.Add(MinerBaseType.EWBF);
                    break;
                case CoinType.ZEN:
                    minerBaseTypeList.Add(MinerBaseType.DSTM);
                    minerBaseTypeList.Add(MinerBaseType.EWBF);
                    break;
                case CoinType.ZCL:
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
                case CoinType.RVN:
                    minerBaseTypeList.Add(MinerBaseType.CCMinerRaven);
                    minerBaseTypeList.Add(MinerBaseType.TRex);
                    minerBaseTypeList.Add(MinerBaseType.WildRig);
                    break;
                case CoinType.SUQA:
                    minerBaseTypeList.Add(MinerBaseType.TRex);
                    minerBaseTypeList.Add(MinerBaseType.CryptoDredge);
                    break;
            }

            return minerBaseTypeList;
        }
    }
}
