using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Models.RPCResponse
{
    public class ClaymoreTemplate
    {
        public List<string> result { get; set; }
        public int id { get; set; }
        public object error { get; set; }
    }

}
