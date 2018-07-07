using BitPoolMiner.Enums;
using BitPoolMiner.Models.CryptoCompare;
using BitPoolMiner.Models.MinerPayments;
using BitPoolMiner.Models.Profitability;
using BitPoolMiner.Models.WhatToMine;
using BitPoolMiner.Utils;
using BitPoolMiner.Utils.CryptoCompare;
using BitPoolMiner.ViewModels.Base;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace BitPoolMiner.ViewModels
{
    public class ProfitabilityViewModel : ViewModelBase
    {
        #region Properties

        private MainWindowViewModel _mainWindowViewModel;

        public WhatToMineData WhatToMineData
        {
            get
            {
                return _mainWindowViewModel.WhatToMineData;
            }
        }

        public MinerPaymentsData MinerPaymentsData
        {
            get
            {
                return _mainWindowViewModel.MinerPaymentsData;
            }
        }

        private ProfitabilityData profitabilityData;
        public ProfitabilityData ProfitabilityData
        {
            get
            {
                return profitabilityData;
            }
            set
            {
                profitabilityData = value;
                OnPropertyChanged();
            }
        }

        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get
            {
                return seriesCollection;
            }
            set
            {
                seriesCollection = value;
                OnPropertyChanged();
            }
        }

        // Charts x axis formatting function
        private Func<double, string> xFormatter;
        public Func<double, string> XFormatter
        {
            get
            {
                return xFormatter;
            }
            set
            {
                xFormatter = value;
                OnPropertyChanged();
            }
        }

        // Charts y axis formatting function
        private Func<double, string> yFormatter;
        public Func<double, string> YFormatter
        {
            get
            {
                return yFormatter;
            }
            set
            {
                yFormatter = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Init

        public ProfitabilityViewModel(MainWindowViewModel mainWindowViewModel)
        {
            // Get a reference back to the main window view model
            _mainWindowViewModel = mainWindowViewModel;
            profitabilityData = new ProfitabilityData();

            PlotPaymentChart();
        }

        #endregion

        #region PaymentChart

        public void PlotPaymentChart()
        {
            try
            {
                // Exit if no fiat currency is selected
                if (Application.Current.Properties["Currency"] == null)
                    return;

                // Create a new list of Miner Payments Grouped by day
                profitabilityData.MinerPaymentsGroupedByDayUnionedList = new List<MinerPaymentsGroupedByDay>();

                // New list of strings used for labels when plotting the chart
                List<string> labels = new List<string>();

                // New collection of series
                seriesCollection = new SeriesCollection();

                foreach (MinerPaymentSummary MinerPaymentSummary in MinerPaymentsData.MinerPaymentSummaryList)
                {
                    StackedAreaSeries stackedColumnSeries = new StackedAreaSeries();
                    ChartValues<DateTimePoint> chartValues = new ChartValues<DateTimePoint>();

                    // Format series fill, stroke, etc
                    FormatChartSeries(MinerPaymentSummary, stackedColumnSeries);

                    // Get CryptoCompare historical rates for the past 30 days
                    HistoDayResponse histoDayResponse = GetCryptoCompare(MinerPaymentSummary.CoinType);

                    // Iterate through each day and add a chart point
                    foreach (MinerPaymentsGroupedByDay minerPaymentsGroupedByDay in MinerPaymentSummary.MinerPaymentsGroupedByDayList.Where(x => x.PaymentDate >= DateTime.Now.AddDays(-30)).OrderByDescending(y => y.PaymentDate))
                    {
                        DateTimePoint dateTimePoint = CalculateDailyPaymentUsingHistoricalRates(MinerPaymentSummary, histoDayResponse, minerPaymentsGroupedByDay);
                        chartValues.Add(dateTimePoint);
                    }

                    labels.Add(MinerPaymentSummary.CoinType.ToString());

                    PaymentChartDataBackFill paymentChartDataBackFill = new PaymentChartDataBackFill();
                    stackedColumnSeries.Values = paymentChartDataBackFill.BackFillList(chartValues);
                    seriesCollection.Add(stackedColumnSeries);
                }

                // Add a line series for the Total Line
                seriesCollection.Add(this.AddTotalLineSeries());

                profitabilityData.MinerPaymentsGroupedByDayUnionedList = profitabilityData.MinerPaymentsGroupedByDayUnionedList.OrderByDescending(x => x.PaymentDate).ToList();

                // Axis label formats
                XFormatter = val => new DateTime((long)val).ToShortDateString();
                YFormatter = val => String.Format("{0} {1}", Math.Round(val, 4).ToString(), Application.Current.Properties["Currency"].ToString());

                OnPropertyChanged("ProfitabilityData");
                OnPropertyChanged("MinerPaymentsData");
                OnPropertyChanged("SeriesCollection");
                OnPropertyChanged("XFormatter");
                OnPropertyChanged("YFormatter");
            }
            catch (Exception e)
            {
                ShowError("Error loading payment data");
            }
        }

        /// <summary>
        /// Format the series based on the particular coin
        /// </summary>
        /// <param name="MinerPaymentSummary"></param>
        /// <param name="stackedColumnSeries"></param>
        private static void FormatChartSeries(MinerPaymentSummary MinerPaymentSummary, StackedAreaSeries stackedColumnSeries)
        {
            // Series color
            var converter = new System.Windows.Media.BrushConverter();

            CoinPaymentChartColor.CoinPaymentChartColorDictionary.TryGetValue(MinerPaymentSummary.CoinType, out string chartColor);

            Brush brushFill = (Brush)converter.ConvertFromString(chartColor);
            brushFill.Opacity = 0.05;

            Brush brushStroke = (Brush)converter.ConvertFromString(chartColor);
            brushStroke.Opacity = 1;

            stackedColumnSeries.Title = MinerPaymentSummary.CoinType.ToString();
            stackedColumnSeries.LineSmoothness = 0.7;
            stackedColumnSeries.PointGeometrySize = 6;
            stackedColumnSeries.Fill = brushFill;

            stackedColumnSeries.Stroke = brushStroke;
            stackedColumnSeries.StrokeThickness = 1;
        }

        /// <summary>
        /// Calculate the total payment amount for each day and create a total line series to plot
        /// </summary>
        /// <returns></returns>
        private LineSeries AddTotalLineSeries()
        {
            LineSeries totalLineSeries = new LineSeries();
            ChartValues<DateTimePoint> chartValues = new ChartValues<DateTimePoint>();

            // Group payments by date and get sum of all fiat payment amounts
            List<MinerPaymentsGroupedByDay> result = profitabilityData.MinerPaymentsGroupedByDayUnionedList
                                    .GroupBy(l => l.PaymentDateTicks)
                                    .Select(cl => new MinerPaymentsGroupedByDay
                                    {
                                        PaymentDateTicks = cl.First().PaymentDateTicks,
                                        PaymentAmountFiat = cl.Sum(c => c.PaymentAmountFiat)
                                    }).ToList();

            // Iterate through each date and add new data points to the line series
            foreach (MinerPaymentsGroupedByDay minerPaymentsGroupedByDay in result)
            {
                DateTimePoint dateTimePoint = new DateTimePoint();
                dateTimePoint.DateTime = minerPaymentsGroupedByDay.PaymentDate;
                dateTimePoint.Value = Convert.ToDouble(minerPaymentsGroupedByDay.PaymentAmountFiat);

                chartValues.Add(dateTimePoint);
            }

            PaymentChartDataBackFill paymentChartDataBackFill = new PaymentChartDataBackFill();
            totalLineSeries.Values = paymentChartDataBackFill.BackFillList(chartValues);

            // Format series
            var converter = new System.Windows.Media.BrushConverter();
            Brush brushStroke = (Brush)converter.ConvertFromString("#FFFFFF");
            brushStroke.Opacity = 0;
            
            totalLineSeries.Title = "TOTAL";
            totalLineSeries.LineSmoothness = 0.7;
            totalLineSeries.PointGeometrySize = 0;

            totalLineSeries.Stroke = brushStroke;
            totalLineSeries.StrokeThickness = 0;

            return totalLineSeries;
        }

        #endregion

        #region CryptoCompare

        /// <summary>
        /// Lookup CryptoCompare data using miner's preferred fiat currency
        /// </summary>
        private HistoDayResponse GetCryptoCompare(CoinType coinType)
        {
            try
            {
                HistoDayResponse histoDayResponse = new HistoDayResponse();

                // Exit if no fiat currency is selected
                if (Application.Current.Properties["Currency"] == null)
                    return new HistoDayResponse();

                string fiatCurrencyISOSymbol = Application.Current.Properties["Currency"].ToString();

                // Load CryptoCompare data
                CryptoCompareAPI cryptoCompareAPI = new CryptoCompareAPI();
                histoDayResponse = cryptoCompareAPI.GetCryptoCompareResponse(coinType.ToString(), fiatCurrencyISOSymbol);
                return histoDayResponse;
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading coin market cap data: {0}", e.Message));
                return new HistoDayResponse();
            }
        }

        /// <summary>
        /// Lookup daily historical rate for each day and calculate payment in users fiat currency
        /// </summary>
        /// <param name="MinerPaymentSummary"></param>
        /// <param name="histoDayResponse"></param>
        /// <param name="minerPaymentsGroupedByDay"></param>
        /// <returns></returns>
        private DateTimePoint CalculateDailyPaymentUsingHistoricalRates(MinerPaymentSummary MinerPaymentSummary, HistoDayResponse histoDayResponse, MinerPaymentsGroupedByDay minerPaymentsGroupedByDay)
        {
            // Lookup CryptoCompare rate for the specific date
            HistoDateResponseData histoDayResponseDay = histoDayResponse.data.Where(x => x.dateTime <= minerPaymentsGroupedByDay.PaymentDate.ToUniversalTime().AddHours(9) &&
                                                                                    x.dateTime >= minerPaymentsGroupedByDay.PaymentDate.ToUniversalTime().AddHours(-9)).FirstOrDefault();

            DateTimePoint dateTimePoint = new DateTimePoint();

            if (histoDayResponseDay == null)
            {
                minerPaymentsGroupedByDay.PaymentAmountFiat = 0;
                dateTimePoint.Value = 0;
                ShowError(String.Format("{0} {1} Missing", minerPaymentsGroupedByDay.PaymentDate.ToUniversalTime(), MinerPaymentSummary.CoinType));
            }
            else
            {
                minerPaymentsGroupedByDay.PaymentAmountFiat = (minerPaymentsGroupedByDay.PaymentAmount * histoDayResponseDay.high);
                dateTimePoint.Value = (double)minerPaymentsGroupedByDay.PaymentAmountFiat;
            }

            // Add payment to complete unioned list that will be bound to the UI
            minerPaymentsGroupedByDay.CoinType = MinerPaymentSummary.CoinType;
            minerPaymentsGroupedByDay.CoinLogo = MinerPaymentSummary.CoinLogo;
            profitabilityData.MinerPaymentsGroupedByDayUnionedList.Add(minerPaymentsGroupedByDay);

            dateTimePoint.DateTime = new DateTime(minerPaymentsGroupedByDay.PaymentDate.Year, minerPaymentsGroupedByDay.PaymentDate.Month, minerPaymentsGroupedByDay.PaymentDate.Day);
            return dateTimePoint;
        }

        #endregion

    }
}
