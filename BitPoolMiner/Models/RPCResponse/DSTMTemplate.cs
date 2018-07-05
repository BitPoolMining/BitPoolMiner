using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Models.RPCResponse
{
    public class DSTMOBjectTemplate
    {
        public int temperature { get; set; }
        public decimal sol_ps { get; set; }
        public int accepted_shares { get; set; }
        public int rejected_shares { get; set; }
        public decimal power_usage { get; set; }
        public int gpu_id { get; set; }
        public string gpu_name { get; set; }
    }

    public class DSTMTemplate
    {
        public int id { get; set; }
        public List<DSTMOBjectTemplate> result { get; set; }
    }
}
