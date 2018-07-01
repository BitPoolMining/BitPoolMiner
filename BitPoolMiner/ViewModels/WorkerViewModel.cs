using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Models.CoinMarketCap;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Utils;
using BitPoolMiner.Utils.CoinMarketCap;
using BitPoolMiner.ViewModels.Base;
using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Timer = System.Timers.Timer;

namespace BitPoolMiner.ViewModels
{
    class WorkerViewModel : ViewModelBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WorkerViewModel()
        {
            InitMonitoringTimer();
            InitMonitorMining24Hour();
            InitMonitorMining();
            InitCoinMarketCap();
        }

        #region Properties

        // Timer for Monitoring Miner
        private Timer MinerStatusCheckTimer;

        // Workername
        private string workerName;
        public string WorkerName
        {
            get
            {
                return workerName;
            }
            set
            {
                workerName = value;
                OnPropertyChanged();
            }
        }

        // Average 24 hour hashrate
        private string hashrate24HourAverage;
        public string Hashrate24HourAverage
        {
            get
            {
                return hashrate24HourAverage;
            }
            set
            {
                hashrate24HourAverage = value;
                OnPropertyChanged();
            }
        }

        // Average 24 hour power
        private string power24HourAverage;
        public string Power24HourAverage
        {
            get
            {
                return power24HourAverage;
            }
            set
            {
                power24HourAverage = value;
                OnPropertyChanged();
            }
        }

        // Efficiency
        private string efficiency;
        public string Efficiency
        {
            get
            {
                return efficiency;
            }
            set
            {
                efficiency = value;
                OnPropertyChanged();
            }
        }

        // List miner stats data
        private MinerMonitorStat minerMonitorStat;
        public MinerMonitorStat MinerMonitorStat
        {
            get
            {
                return minerMonitorStat;
            }
            set
            {
                minerMonitorStat = value;
                OnPropertyChanged();
            }
        }

        // List of 24 hour miner stats data
        private ObservableCollection<MinerMonitorStat> minerMonitorStatList24Hour;
        public ObservableCollection<MinerMonitorStat> MinerMonitorStatList24Hour
        {
            get
            {
                return minerMonitorStatList24Hour;
            }
            set
            {
                minerMonitorStatList24Hour = value;
                OnPropertyChanged();
            }
        }

        // Chart Hashrate Series
        private ChartValues<DateTimePoint> chartValuesHashRate;
        public ChartValues<DateTimePoint> ChartValuesHashRate
        {
            get
            {
                return chartValuesHashRate;
            }
            set
            {
                chartValuesHashRate = value;
                OnPropertyChanged();
            }
        }

        // Chart Power Series
        private ChartValues<DateTimePoint> chartValuesPower;
        public ChartValues<DateTimePoint> ChartValuesPower
        {
            get
            {
                return chartValuesPower;
            }
            set
            {
                chartValuesPower = value;
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

        // CoinMarketCap data
        private CoinMarketCapResponse coinMarketCapResponse;
        public CoinMarketCapResponse CoinMarketCapResponse
        {
            get
            {
                return coinMarketCapResponse;
            }
            set
            {
                coinMarketCapResponse = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Init

        /// <summary>
        /// Init timer used for monitoring and mining stats
        /// </summary>
        private void InitMonitoringTimer()
        {
            MinerStatusCheckTimer = new Timer();
            MinerStatusCheckTimer.Elapsed += MinerStatusCheckTimer_Elapsed;
            MinerStatusCheckTimer.Interval = 30000;
            MinerStatusCheckTimer.Enabled = true;
        }

        #endregion

        #region Monitoring

        /// <summary>
        /// Monitoring timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MinerStatusCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Call miner RPC and post results to API
            InitMonitorMining24Hour();
            InitMonitorMining();
            InitCoinMarketCap();
        }

        /// <summary>
        /// Load list of miner monitor stats
        /// </summary>
        public void InitMonitorMining24Hour()
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null && WorkerName != null)
                {
                    // Load list of miner monitor stats
                    MinerMonitorStatsAPI minerMonitorStatsAPI = new MinerMonitorStatsAPI();
                    MinerMonitorStatList24Hour = minerMonitorStatsAPI.GetMinerMonitorStats24Hour();

                    // LineChart data
                    ChartValuesHashRate = new ChartValues<DateTimePoint>();
                    ChartValuesPower = new ChartValues<DateTimePoint>();

                    // Filter list by worker
                    List<MinerMonitorStat> MinerMonitorStatListFiltered = new List<MinerMonitorStat>();
                    MinerMonitorStatListFiltered = MinerMonitorStatList24Hour.Where(x => x.WorkerName == WorkerName).OrderBy(y => y.Created).ToList();

                    // Calculate average values and display
                    if (MinerMonitorStatListFiltered.Count > 0)
                    {
                        Calculate24HourAverageHashrate(MinerMonitorStatListFiltered);
                        Calculate24HourAveragePower(MinerMonitorStatListFiltered);
                    }
                    else
                    {
                        Hashrate24HourAverage = "0";
                        Power24HourAverage = "0";
                    }

                    // Populate series to be graphed
                    foreach (MinerMonitorStat minerMonitorStat in MinerMonitorStatListFiltered)
                    {
                        DateTimePoint dateTimePoint = new DateTimePoint();

                        // HashRate
                        dateTimePoint.DateTime = minerMonitorStat.Created.ToLocalTime();
                        dateTimePoint.Value = HashrateFormatter.FormatNumeric(minerMonitorStat.CoinType, minerMonitorStat.HashRate);
                        ChartValuesHashRate.Add(dateTimePoint);

                        // Power
                        DateTimePoint dateTimePointPower = new DateTimePoint();
                        dateTimePointPower.DateTime = minerMonitorStat.Created.ToLocalTime();
                        dateTimePointPower.Value = minerMonitorStat.Power;
                        ChartValuesPower.Add(dateTimePointPower);
                    }

                    // Backfill lists as needed
                    WorkerChartDataBackFill chartDataBackFill = new WorkerChartDataBackFill();
                    ChartValuesHashRate = chartDataBackFill.BackFillList(ChartValuesHashRate);
                    ChartValuesPower = chartDataBackFill.BackFillList(ChartValuesPower);

                    // Axis label formats
                    XFormatter = val => new DateTime((long)val).ToString();
                    YFormatter = val => val.ToString();

                    // Notify UI of change
                    OnPropertyChanged("ChartValuesHashRate");
                    OnPropertyChanged("ChartValuesPower");
                }
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading monitor data: {0}", e.Message));
            }
        }

        /// <summary>
        /// Load list of miner monitor stats
        /// </summary>
        public void InitMonitorMining()
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null && WorkerName != null)
                {
                    // Load list of miner monitor stats
                    MinerMonitorStatsAPI minerMonitorStatsAPI = new MinerMonitorStatsAPI();
                    ObservableCollection<MinerMonitorStat> MinerMonitorStatList = minerMonitorStatsAPI.GetMinerMonitorStats();

                    // Filter list by worker
                    MinerMonitorStat = MinerMonitorStatList.Where(x => x.WorkerName == WorkerName).OrderBy(y => y.Created).FirstOrDefault();

                    // Calculate Efficiency
                    if (MinerMonitorStat.Power <= 0 || MinerMonitorStat.HashRate <= 0)
                    {
                        Efficiency = "0";
                    }
                    else
                    {
                        Efficiency = String.Format("{0}/W", HashrateFormatter.Format(MinerMonitorStat.CoinType, (MinerMonitorStat.HashRate / MinerMonitorStat.Power)));
                    }

                    // Notify UI of change
                    OnPropertyChanged("MinerMonitorStat");
                    OnPropertyChanged("Efficiency");
                }
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading monitor data: {0}", e.Message));
            }
        }

        /// <summary>
        /// Calculate 24 Hour Average Hashrate
        /// </summary>
        /// <param name="MinerMonitorStatList"></param>
        private void Calculate24HourAverageHashrate(List<MinerMonitorStat> MinerMonitorStatList)
        {
            Hashrate24HourAverage = HashrateFormatter.Format(MinerMonitorStatList.FirstOrDefault().CoinType, MinerMonitorStatList.Average(x => x.HashRate));

            // Notify UI of change
            OnPropertyChanged("Hashrate24HourAverage");
        }

        /// <summary>
        /// Calculate 24 Hour Average Power
        /// </summary>
        /// <param name="MinerMonitorStatList"></param>
        private void Calculate24HourAveragePower(List<MinerMonitorStat> MinerMonitorStatList)
        {
            Power24HourAverage = String.Format("{0} W", Convert.ToInt32(MinerMonitorStatList.Average(x => x.Power)));

            // Notify UI of change
            OnPropertyChanged("Power24HourAverage");
        }

        #endregion

        #region CoinMarketCap

        /// <summary>
        /// Lookup CoinMarketCap data using miner's preferred fiat currency
        /// </summary>
        public void InitCoinMarketCap()
        {
            try
            {
                // Exit if no fiat currency is selected
                if (Application.Current.Properties["Currency"] == null)
                    return;

                // Exit if select worker is not currently mining
                if (MinerMonitorStat == null || MinerMonitorStat.CoinType == CoinType.UNDEFINED)
                    return;

                string fiatCurrencyISOSymbol = Application.Current.Properties["Currency"].ToString();

                // Attempt to get crypto coin name
                CoinNames.CoinNameDictionary.TryGetValue(MinerMonitorStat.CoinType, out string cryptoCurrencyName);

                // Load CoinMarketCap data
                CoinMarketCapAPI coinMarketCapAPI = new CoinMarketCapAPI();
                CoinMarketCapResponse = coinMarketCapAPI.GetCoinMarketCapResponse(cryptoCurrencyName, fiatCurrencyISOSymbol);
                OnPropertyChanged("CoinMarketCapResponse");
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading coin market cap data: {0}", e.Message));
            }
        }

        #endregion

    }
}
