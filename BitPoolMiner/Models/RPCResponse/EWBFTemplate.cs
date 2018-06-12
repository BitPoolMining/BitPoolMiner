using System.Collections.Generic;

namespace BitPoolMiner.Models.RPCResponse
{
    public class EWBFOBjectTemplate
    {
        public int temperature { get; set; }
        public int speed_sps { get; set; }
        public int accepted_shares { get; set; }
        public int rejected_shares { get; set; }
        public int gpu_power_usage { get; set; }
        public int gpuid { get; set; }
    }

    public class EWBFTemplate
    {
        public int id { get; set; }
        public List<EWBFOBjectTemplate> result { get; set; }
    }
}
