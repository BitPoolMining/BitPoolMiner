using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Models.RPCResponse
{
    public class TRexTemplate
    {
        //public int accepted_count  { get; set; }

        //"active_pool": {                           ----- Information about the pool your miner is currently connected to.
        //	"difficulty": 5,                       ----- Current pool difficulty.
        //	"ping": 97,                            ----- Pool latency.
        //	"retries": 0,                          ----- Number of connection attempts in case of connection loss.
        //	"url": "stratum+tcp://...",            ----- Pool connection string.
        //	"user": "..."                          ----- Usually your wallet address.

        //   },

        //"algorithm": "x16r",                       ----- Algorithm which was set in config.
        //"api": "1.2",                              ----- HTTP API protocol version.
        //"cuda": "9.10",                            ----- CUDA library version used.
        //"description": "T-Rex NVIDIA GPU miner",
        //"difficulty": 31968.245093004043,          ----- Current network difficulty.
        //"gpu_total": 1,                            ----- Total number of GPUs installed in your system.

        public List<GPUList> gpus { get; set; }

        public int hashrate { get; set; }
        public int hashrate_day { get; set; }
        public int hashrate_hour { get; set; }
        public int hashrate_minute { get; set; }
        public string name { get; set; }
        public string os { get; set; }
        public int rejected_count { get; set; }
        public int solved_count { get; set; }
        public int ts { get; set; }
        public int uptime { get; set; }
        public string version { get; set; }
    }

    public class GPUList
    {
        public int device_id { get; set; }
        public int fan_speed { get; set; }
        public int gpu_id { get; set; }
        public int hashrate { get; set; }
        public int hashrate_day { get; set; }
        public int hashrate_hour { get; set; }
        public int hashrate_minute { get; set; }
        public decimal intensity { get; set; }
        public string name { get; set; }
        public int temperature { get; set; }
        public string vendor { get; set; }
        public bool disabled { get; set; }
        public int power { get; set; }
        public int disabled_at_temperature { get; set; }
    }

}

