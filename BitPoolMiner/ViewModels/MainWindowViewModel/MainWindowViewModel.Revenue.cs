using BitPoolMiner.Enums;
using BitPoolMiner.Formatter;
using BitPoolMiner.Models;
using BitPoolMiner.Models.MinerPayments;
using BitPoolMiner.Models.WhatToMine;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Utils.WhatToMine;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;

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

        private MinerPaymentsData minerPaymentsData;
        public MinerPaymentsData MinerPaymentsData
        {
            get
            {
                return minerPaymentsData;
            }
            set
            {
                minerPaymentsData = value;
                OnPropertyChanged("MinerPaymentsData");
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

            whatToMineData.WhatToMineResponseList.OrderBy(x => x.name).ToList();

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
            whatToMineData.ForecastNext24HourBTC = Decimal.Round(whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.btc_revenue)), 6);
            whatToMineData.ForecastNext24HourCoin = Decimal.Round(whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.estimated_rewards)), 6);
        }

        #endregion

        #region Payments

        public void InitPayments()
        {
            if (Application.Current.Properties["AccountID"] != null)
            {
                // Instantiate new Payment data
                minerPaymentsData = new MinerPaymentsData();
                List<MinerPaymentSummary> minerPaymentsList = new List<MinerPaymentSummary>();

                // Load payment data from the API
                MinerPaymentsAPI minerPaymentsAPI = new MinerPaymentsAPI();
                minerPaymentsList = minerPaymentsAPI.GetMinerPayments();

                // Add payment data to list
                minerPaymentsData.MinerPaymentSummaryList = minerPaymentsList.OrderBy(x => x.CoinType).ToList();

                // Calculate 24 hour sum values
                CalculateRevenueLast24Hour();

                OnPropertyChanged("MinerPaymentsData");
            }
        }

        /// <summary>
        /// Populate the payment summarized revenue for the next 24 hours for all coins and also for each individual coin
        /// </summary>
        private void CalculateRevenueLast24Hour()
        {
            // Iterate through each payment summary in the list for each coin
            foreach (MinerPaymentSummary minerPaymentSummary in minerPaymentsData.MinerPaymentSummaryList)
            {
                // Update coin logo for each miner
                CoinLogos.CoinLogoDictionary.TryGetValue(minerPaymentSummary.CoinType, out string logoSourceLocation);
                if (minerPaymentSummary.CoinType != CoinType.UNDEFINED)
                    minerPaymentSummary.CoinLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logoSourceLocation);

                // Find W2M record to get exchange rate data
                WhatToMineResponse whatToMineResponse = WhatToMineData.WhatToMineResponseList.Where(x => x.tag == minerPaymentSummary.CoinType.ToString()).FirstOrDefault();

                // If there is no W2M data then do not try to convert rates
                if (whatToMineResponse == null)
                {
                    minerPaymentSummary.RevenueLast24HourCoin = Decimal.Round(minerPaymentSummary.MinerPaymentDetails24HoursList.Sum(x => x.PaymentAmount), 6);
                    minerPaymentSummary.RevenueLast24HourBTC = 0;
                    minerPaymentSummary.RevenueLast24HourUSD = 0;
                }
                else
                {
                    minerPaymentSummary.RevenueLast24HourCoin = Decimal.Round(minerPaymentSummary.MinerPaymentDetails24HoursList.Sum(x => x.PaymentAmount), 6);
                    minerPaymentSummary.RevenueLast24HourBTC = Decimal.Round(minerPaymentSummary.RevenueLast24HourCoin * Convert.ToDecimal(whatToMineResponse.exchange_rate), 6);
                    minerPaymentSummary.RevenueLast24HourUSD = 0;
                }
            }

            // Calculate sum revenue across all coins
            minerPaymentsData.RevenueLast24HourUSD = minerPaymentsData.MinerPaymentSummaryList.Sum(x => x.RevenueLast24HourUSD);
            minerPaymentsData.RevenueLast24HourBTC = minerPaymentsData.MinerPaymentSummaryList.Sum(x => x.RevenueLast24HourBTC);
        }

        #endregion
    }
}
