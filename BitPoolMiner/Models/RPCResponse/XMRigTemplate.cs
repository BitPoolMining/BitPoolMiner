using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitPoolMiner.Models.RPCResponse
{
    public class XMRigTemplate
    {
        public hashrate hashrate { get; set; }
        public cpu cpu { get; set; }
    }

    public class hashrate
    {
        public double highest { get; set; }
    }

    public class cpu
    {
        public string brand { get; set; }
    }
}

