using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Models.CoinMarketCap
{
    public class CoinMarketCapResponse
    {
        public string fiat_currency_iso_symbol { get; set; }
        public decimal price_btc { get; set; }
        public decimal price_fiat { get; set; }
        public decimal volume_fiat_24h { get; set; }
        public decimal market_cap_fiat { get; set; }
        public decimal percent_change_24h { get; set; }
        public decimal percent_change_7d { get; set; }                
    }
}
