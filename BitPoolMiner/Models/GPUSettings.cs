using BitPoolMiner.Enums;
using BitPoolMiner.ViewModels.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        [DefaultValue(CoinType.UNDEFINED)]
        [JsonConverter(typeof(StringEnumConverter))]
        public CoinType CoinSelectedForMining
        {
            get
            {
                return coinSelectedForMining; ;
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
                coinTypeList.Add(CoinType.ETC);
                coinTypeList.Add(CoinType.XMR);
                coinTypeList.Add(CoinType.RVN);
                coinTypeList.Add(CoinType.VTC);
            }
            else if (HardwareType == HardwareType.Nvidia)
            {
                coinTypeList.Add(CoinType.ETC);
                coinTypeList.Add(CoinType.VTC);
                coinTypeList.Add(CoinType.RVN);
                coinTypeList.Add(CoinType.XMR);
            }
            else if (HardwareType == HardwareType.CPU)
            {
                coinTypeList.Add(CoinType.XMR);
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
            switch (coinSelectedForMining)
            {
                case CoinType.ETC:
                    minerBaseTypeList.Add(MinerBaseType.Claymore);
                    break;
                case CoinType.VTC:
                    minerBaseTypeList.Add(MinerBaseType.CCMiner);
                    minerBaseTypeList.Add(MinerBaseType.CryptoDredge);
                    minerBaseTypeList.Add(MinerBaseType.LyclMiner);
                    break;
                case CoinType.RVN:
                    minerBaseTypeList.Add(MinerBaseType.CryptoDredge);
                    minerBaseTypeList.Add(MinerBaseType.TRex);
                    minerBaseTypeList.Add(MinerBaseType.WildRig);
                    break;
                case CoinType.XMR:
                    minerBaseTypeList.Add(MinerBaseType.XMRig);
                    minerBaseTypeList.Add(MinerBaseType.XMRigAMD);
                    minerBaseTypeList.Add(MinerBaseType.XMRigNvidia);
                    break;
            }

            return minerBaseTypeList;
        }
    }
}
