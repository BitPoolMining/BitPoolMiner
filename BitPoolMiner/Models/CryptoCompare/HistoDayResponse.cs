using System;
using System.Collections.Generic;

namespace BitPoolMiner.Models.CryptoCompare
{
    /// <summary>
    /// Response from CryptoCompany for the HistoDay endpoint
    /// </summary>
    public class HistoDayResponse
    {
        public string Response { get; set; }
        public int Type { get; set; }
        public bool Aggregated { get; set; }
        public List<HistoDateResponseData> data { get; set; }
    }

    /// <summary>
    /// Response from CryptoCompany for the HistoDay endpoint representing the daily data
    /// </summary>
    public class HistoDateResponseData
    {
        // Unix timestamp is seconds past epoch
        DateTime unixDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public long time { get; set; }
        public decimal high { get; set; }
        public decimal low { get; set; }
        public decimal open { get; set; }
        public decimal volumefrom { get; set; }
        public decimal volumeto { get; set; }
        public decimal close { get; set; }
        public DateTime dateTime
        {
            get
            {
                return unixDateTime.AddSeconds(time);
            }
        }
    }
}
