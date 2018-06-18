using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Utils;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace BitPoolMiner.ViewModels
{
    /// <summary>
    /// Handles all monitoring and UI related tasks for the Miner and GPU monitoring statistics
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region Init

        /// <summary>
        /// Init timer used for monitoring and mining stats
        /// </summary>
        private void InitMonitoringCheckTimer()
        {
            // Immediately get results before time is instantiated
            GetMinerMonitoringResults();
            GetAccountWorkerList();

            // Instantiate and start timer
            MinerStatusCheckTimer = new DispatcherTimer();
            MinerStatusCheckTimer.Tick += MinerStatusCheckTimer_Elapsed;
            MinerStatusCheckTimer.Interval = TimeSpan.FromSeconds(60);  // 60 second default right now.  EWBF won't display any data until it submits the first share.
            MinerStatusCheckTimer.Start();
        }

        #endregion

        #region Properties

        // Timer for Monitoring Miner
        private DispatcherTimer MinerStatusCheckTimer;

        // Miner monitor stats grouped by coin 
        private ObservableCollection<MinerMonitorStat> minerMonitorStatListGrouped;
        public ObservableCollection<MinerMonitorStat> MinerMonitorStatListGrouped
        {
            get
            {
                return minerMonitorStatListGrouped;
            }
            set
            {
                minerMonitorStatListGrouped = value;
                OnPropertyChanged("MinerMonitorStatListGrouped");
            }
        }

        // Workers online
        private string workersOnline;
        public string WorkersOnline
        {
            get
            {
                return workersOnline;
            }
            set
            {
                workersOnline = value;
                OnPropertyChanged("WorkersOnline");
            }
        }

        // Workers offline
        private string workersOffline;
        public string WorkersOffline
        {
            get
            {
                return workersOffline;
            }
            set
            {
                workersOffline = value;
                OnPropertyChanged("WorkersOffline");
            }
        }

        // Total power
        private string totalPower;
        public string TotalPower
        {
            get
            {
                return totalPower;
            }
            set
            {
                totalPower = value;
                OnPropertyChanged("TotalPower");
            }
        }

        #endregion

        #region Monitoring

        /// <summary>
        /// Monitoring timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MinerStatusCheckTimer_Elapsed(object sender, EventArgs e)
        {
            // Call miner RPC and post results to API
            GetMinerMonitoringResults();
            GetAccountWorkerList();
        }

        /// <summary>
        /// Load list of miner monitor stats
        /// </summary>
        public void GetMinerMonitoringResults()
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null)
                {
                    // Load list of miner monitor stats
                    List<MinerMonitorStat> minerMonitorStatList = new List<MinerMonitorStat>();
                    MinerMonitorStatsAPI minerMonitorStatsAPI = new MinerMonitorStatsAPI();
                    minerMonitorStatList = minerMonitorStatsAPI.GetMinerMonitorStats().ToList();
                    GetMinerMonitoringSumGrouped(minerMonitorStatList);
                    GetWorkersOnlineGrouped(minerMonitorStatList);
                    GetWorkersOfflineGrouped(minerMonitorStatList);
                    GetTotalPower(minerMonitorStatList);
                }
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading monitor data: {0}", e.Message));
            }
        }

        /// <summary>
        /// Group stats by coin to show details
        /// </summary>
        /// <param name="minerMonitorStatList"></param>
        private void GetMinerMonitoringSumGrouped(List<MinerMonitorStat> minerMonitorStatList)
        {
            // Populate properties for UI binding
            // Group stats by coin to show details
            List<MinerMonitorStat> minerMonitorStatListGrouped = minerMonitorStatList
                .Where(x => x.CoinType != CoinType.UNDEFINED)
                .GroupBy(l => l.CoinType)
                .Select(cl => new MinerMonitorStat
                {
                    CoinLogo = cl.First().CoinLogo,
                    CoinType = cl.First().CoinType,
                    CountStats = cl.Count(),
                    HashRate = cl.Sum(c => c.HashRate)
                }).ToList();

            // Format the hashrate of each grouped sum of hashrate per cointype
            foreach (MinerMonitorStat minerMonitorStat in minerMonitorStatListGrouped)
            {
                minerMonitorStat.DisplayHashRate = HashrateFormatter.Format(minerMonitorStat.CoinType, minerMonitorStat.HashRate);
            }

            MinerMonitorStatListGrouped = new ObservableCollection<MinerMonitorStat>(minerMonitorStatListGrouped);

            // Notify UI of change
            OnPropertyChanged("MinerMonitorStatListGrouped");
        }

        /// <summary>
        /// Display count of online workers
        /// </summary>
        /// <param name="minerMonitorStatList"></param>
        private void GetWorkersOnlineGrouped(List<MinerMonitorStat> minerMonitorStatList)
        {
            WorkersOnline = String.Format("{0} Workers", minerMonitorStatList.Where(x => x.Status.ToLower() == "online").ToList().Count);

            // Notify UI of change
            OnPropertyChanged("WorkersOnline");
        }

        /// <summary>
        /// Display count of offline workers
        /// </summary>
        /// <param name="minerMonitorStatList"></param>
        private void GetWorkersOfflineGrouped(List<MinerMonitorStat> minerMonitorStatList)
        {
            WorkersOffline = String.Format("{0} Workers", minerMonitorStatList.Where(x => x.Status.ToLower() != "online").ToList().Count);

            // Notify UI of change
            OnPropertyChanged("WorkersOffline");
        }

        /// <summary>
        /// Group summarized total power
        /// </summary>
        /// <param name="minerMonitorStatList"></param>
        private void GetTotalPower(List<MinerMonitorStat> minerMonitorStatList)
        {
            TotalPower = String.Format("{0}W", minerMonitorStatList.Sum(x => x.Power));

            // Notify UI of change
            OnPropertyChanged("TotalPower");
        }

        #endregion
    }
}
