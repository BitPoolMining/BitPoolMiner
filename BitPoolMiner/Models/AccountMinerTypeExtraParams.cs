using BitPoolMiner.Enums;
using Newtonsoft.Json;

namespace BitPoolMiner.Models
{
    public class AccountMinerTypeExtraParams
    {
        /// <summary>
        /// Miner Type to Add Extra Param to
        /// </summary>
        public string MinerBaseTypeString { get; set; }

        /// <summary>
        /// Miner Type to Add Extra Param to
        /// </summary>
        [JsonIgnore]
        public MinerBaseType MinerBaseType
        {
            get
            {
                MinerBaseType minerBaseType;
                if(System.Enum.TryParse(MinerBaseTypeString, out minerBaseType))
                {
                    return minerBaseType;
                }
                else
                {
                    return MinerBaseType.UNDEFINED;
                }
            }

        }

        /// <summary>
        /// Extra miner parameters to use
        /// </summary>
        public string ExtraParams { get; set; }
    }
}
