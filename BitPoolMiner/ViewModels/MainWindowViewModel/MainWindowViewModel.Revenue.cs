using BitPoolMiner.Enums;
using BitPoolMiner.Formatter;
using BitPoolMiner.Models;
using BitPoolMiner.Models.CoinMarketCap;
using BitPoolMiner.Models.MinerPayments;
using BitPoolMiner.Models.WhatToMine;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Utils.CoinMarketCap;
using BitPoolMiner.Utils.WhatToMine;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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

            // Iterate through each miner that has monitor stats
            foreach (MinerMonitorStat minerMonitorStat in MinerMonitorStatListGrouped)
            {
                // Retrieve forecasts from W2M
                WhatToMineResponse whatToMineResponse = GetWhatToMineEstimates(minerMonitorStat);
                whatToMineResponse = WhatToMineDataFormatter.FormatWhatToMineData(whatToMineResponse, minerMonitorStat.CoinType);

                // Retrieve the current rates from CoinMarketCap for this particular coin and user's selected fiat
                CoinMarketCapResponse coinMarketCapResponse = GetCoinMarketCapData(minerMonitorStat.CoinType);

                whatToMineResponse.Revenue = Decimal.Round(Convert.ToDecimal(whatToMineResponse.Estimated_rewards) * Convert.ToDecimal(coinMarketCapResponse.price_fiat), 2).ToString();

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

            // This is invariant, always use USD based formatting for W2M calls
            NumberFormatInfo format = new NumberFormatInfo();
            // Set the 'splitter' for thousands
            format.NumberGroupSeparator = ",";
            // Set the decimal seperator
            format.NumberDecimalSeparator = ".";

            if (minerMonitorStat.CoinType == CoinType.VTC)
            {
                nameValueCollection.Add("hr", (minerMonitorStat.HashRate / 1000).ToString(format));
            }
            else if (minerMonitorStat.CoinType == CoinType.ETC || minerMonitorStat.CoinType == CoinType.RVN)
            {
                // Expects MH/s
                nameValueCollection.Add("hr", (minerMonitorStat.HashRate / 1000000).ToString(format));
            }
            else
            {
                nameValueCollection.Add("hr", minerMonitorStat.HashRate.ToString(format));
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
            whatToMineData.ForecastNext24HourUSD = whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.Revenue.ToString().Replace("$", "")));
            whatToMineData.ForecastNext24HourBTC = Decimal.Round(whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.BTC_revenue)), 6);
            whatToMineData.ForecastNext24HourCoin = Decimal.Round(whatToMineData.WhatToMineResponseList.Sum(x => Convert.ToDecimal(x.Estimated_rewards)), 6);
        }

        #endregion

        #region Payments

        public void InitPayments()
        {
            try
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
                    minerPaymentsData.MinerPaymentSummaryList = minerPaymentsList.OrderBy(x => x.CoinType.ToString()).ToList();

                    // Calculate 24 hour sum values
                    CalculateRevenueLast24Hour();

                    OnPropertyChanged("MinerPaymentsData");
                }
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading payment history: {0}", e.Message));
            }
        }

        /// <summary>
        /// Populate the payment summarized revenue for the next 24 hours for all coins and also for each individual coin
        /// </summary>
        private void CalculateRevenueLast24Hour()
        {
            try
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
                        // Retrieve the current rates from CoinMarketCap for this particular coin and user's selected fiat
                        CoinMarketCapResponse coinMarketCapResponse = GetCoinMarketCapData(minerPaymentSummary.CoinType);

                        minerPaymentSummary.RevenueLast24HourCoin = Decimal.Round(minerPaymentSummary.MinerPaymentDetails24HoursList.Sum(x => x.PaymentAmount), 6);
                        minerPaymentSummary.RevenueLast24HourBTC = Decimal.Round(minerPaymentSummary.RevenueLast24HourCoin * Convert.ToDecimal(whatToMineResponse.exchange_rate), 6);
                        minerPaymentSummary.RevenueLast24HourUSD = Decimal.Round(minerPaymentSummary.RevenueLast24HourCoin * Convert.ToDecimal(coinMarketCapResponse.price_fiat), 2); ;
                    }
                }

                // Calculate sum revenue across all coins
                minerPaymentsData.RevenueLast24HourUSD = minerPaymentsData.MinerPaymentSummaryList.Sum(x => x.RevenueLast24HourUSD);
                minerPaymentsData.RevenueLast24HourBTC = minerPaymentsData.MinerPaymentSummaryList.Sum(x => x.RevenueLast24HourBTC);

                // Order actual payment history
                minerPaymentsData.MinerPaymentSummaryList = minerPaymentsData.MinerPaymentSummaryList.OrderBy(x => x.CoinType.ToString()).ToList();
                OnPropertyChanged("MinerPaymentsData");
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error calculating last 24 hour payment data {0}", e.Message));
            }
        }

        /// <summary>
        /// Get CoinMarketCap info for conversion using specified fiat currency for worker
        /// </summary>
        /// <param name="coinType"></param>
        private CoinMarketCapResponse GetCoinMarketCapData(CoinType coinType)
        {
            try
            {
                // Exit if no fiat currency is selected
                if (Application.Current.Properties["Currency"] == null)
                    return new CoinMarketCapResponse();

                string fiatCurrencyISOSymbol = Application.Current.Properties["Currency"].ToString();

                // Attempt to get crypto coin name
                CoinNames.CoinNameDictionary.TryGetValue(coinType, out string cryptoCurrencyName);

                // Load CoinMarketCap data
                CoinMarketCapAPI coinMarketCapAPI = new CoinMarketCapAPI();
                CoinMarketCapResponse coinMarketCapResponse = coinMarketCapAPI.GetCoinMarketCapResponse(cryptoCurrencyName, fiatCurrencyISOSymbol);

                return coinMarketCapResponse;
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error retrieving coin market cap data: {0}", e.Message));
                return new CoinMarketCapResponse();
            }
        }

        #endregion
    }
}
