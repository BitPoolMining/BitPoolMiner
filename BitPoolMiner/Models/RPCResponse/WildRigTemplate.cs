using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Models.RPCResponse
{
    public class WildRigTemplate
    { 
        public WildRigHashrate hashrate { get; set; }
    }

    public class WildRigHashrate
    {
        public List<int> total { get; set; }
        public int highest { get; set; }
        public List<List<int>> threads { get; set; }
    }
}

