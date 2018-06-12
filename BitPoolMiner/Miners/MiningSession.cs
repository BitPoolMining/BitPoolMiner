using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Miners
{
    public class MiningSession
    {
        private readonly List<Miner> Miners = new List<Miner>();


        // The logic of adding the miners will change as we add the ability to launch multiple miners for the same machine.

        public void AddMiner(Miner m)
        {
            Miners.Add(m);
        }

        public void RemoveAllMiners()
        {
            StopMiningSession();
            Miners.Clear();
        }

        public void StartMiningSession()
        {
            foreach (Miner m in Miners)
            {
                m.Start();
            }
        }

        public void StopMiningSession()
        {
            foreach (Miner m in Miners)
            {
                m.Stop();
            }
        }

        public void GetMinerStatsAsync()
        {
            foreach (Miner m in Miners)
            {
                m.ReportStatsAsyc();
            }
        }
    }
}
