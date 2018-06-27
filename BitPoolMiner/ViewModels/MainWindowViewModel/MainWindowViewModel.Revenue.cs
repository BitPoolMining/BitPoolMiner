using BitPoolMiner.Enums;
using BitPoolMiner.Formatter;
using BitPoolMiner.Models;
using BitPoolMiner.Models.WhatToMine;
using BitPoolMiner.Utils.WhatToMine;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace BitPoolMiner.ViewModels
{
    /// <summary>
    /// Handles all data related to payments and forecasting
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region Properties

        private WhatToMineData whatToMineData;
        public WhatToMineData WhatToMineData
        {
            get
            {
                return whatToMineData;
            }
            set
            {
                whatToMineData = value;
                OnPropertyChanged("WhatToMineData");
            }
        }

        #endregion

        #region WhatToMine

        public void InitWhatToMine()
        {
            // Instantiate new W2M objects
            whatToMineData = new WhatToMineData();
            whatToMineData.WhatToMineResponseList = new List<WhatToMineResponse>();

            // Make sure that Monitor stat list is not null
            if (MinerMonitorStatListGrouped == null)
                return;

            foreach (MinerMonitorStat minerMonitorStat in MinerMonitorStatListGrouped)
            {
                WhatToMineResponse whatToMineResponse = GetWhatToMineEstimates(minerMonitorStat);
                whatToMineResponse = WhatToMineDataFormatter.FormatWhatToMineData(whatToMineResponse, minerMonitorStat.CoinType);
                whatToMineData.WhatToMineResponseList.Add(whatToMineResponse);
            }

            CalculateForecastLast24Hour();
            CalculateForecastNext24Hour();

            OnPropertyChanged("WhatToMineData");
        }

        /// <summary>
        /// Get responses from 
        /// </summary>
        /// <param name="minerMonitorStat"></param>
        /// <returns></returns>
        private WhatToMineResponse GetWhatToMineEstimates(MinerMonitorStat minerMonitorStat)
        {
            // Build up Query Parameters for API request
            NameValueCollection nameValueCollection = new NameValueCollection();

            if (minerMonitorStat.CoinType == CoinType.MONA || minerMonitorStat.CoinType == CoinType.VTC)
            {
                nameValueCollection.Add("hr", (minerMonitorStat.HashRate / 1000).ToString());
            }
            else
            {
                nameValueCollection.Add("hr", minerMonitorStat.HashRate.ToString());
            }
            
            nameValueCollection.Add("p", minerMonitorStat.Power.ToString());
            nameValueCollection.Add("fee", "0.05");         // Pool Fee
            nameValueCollection.Add("cost", "0.1");         // Elec cost per kwh
            nameValueCollection.Add("hcost", "0.0");        // Hardware costs
            nameValueCollection.Add("commit", "Calculate"); // Calc
            nameValueCollection.Add("revenue", "24h");      // 24 hour results

            // Load WhatToMine forecasts
            WhatToMineResponse whatToMineResponse = new WhatToMineResponse();
            WhatToMineAPI whatToMineAPI = new WhatToMineAPI();
            whatToMineResponse = whatToMineAPI.GetWhatToMineEstimates(minerMonitorStat.CoinType, nameValueCollection);
            return whatToMineResponse;
        }

        /// <summary>
        /// Populate the WhatToMine summarized forecast for the last 24 hours
        /// </summary>
        private void CalculateForecastLast24Hour()
        {
            whatToMineData.ForecastLast24HourUSD = 0;
            whatToMineData.ForecastLast24HourBTC = 0;
            whatToMineData.ForecastLast24HourCoin = 0;
        }

        /// <summary>
        /// Populate the WhatToMine summarized forecast for the next 24 hours
        /// </summary>
        private void CalculateForecastNext24Hour()
        {
            whatToMineData.ForecastNext24HourUSD = whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.revenue.ToString().Replace("$","")));
            whatToMineData.ForecastNext24HourBTC = whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.btc_revenue));
            whatToMineData.ForecastNext24HourCoin = whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.estimated_rewards));
        }

        #endregion

    }
}
