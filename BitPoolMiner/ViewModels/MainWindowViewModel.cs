using BitPoolMiner.Enums;
using BitPoolMiner.Miners;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Utils;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Timer = System.Timers.Timer;

namespace BitPoolMiner.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Init

        public MainWindowViewModel()
        {
            // Wire up commands for mining
            CommandStartMining = new RelayCommand(StartAllMiners);
            CommandStopMining = new RelayCommand(StopAllMiners);

            // Initialize objects
            InitMining();
            InitMonitoringCheckTimer();

            // Init account worker list
            AccountWorkersList = new ObservableCollection<AccountWorkers>();

            // Get a reference to the MainView window
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        /// <summary>
        /// Initial objects
        /// </summary>
        private void InitMining()
        {
            // Init timer used for monitoring and mining stats
            MinerStatusInsertTimer = new Timer();
            MinerStatusInsertTimer.Elapsed += MinerStatusInsertTimer_Elapsed;
            MinerStatusInsertTimer.Interval = 30000;  // 30 second default right now.  EWBF won't display any data until it submits the first share.
            MinerStatusInsertTimer.Enabled = false;
        }

        /// <summary>
        /// Init timer used for monitoring and mining stats
        /// </summary>
        private void InitMonitoringCheckTimer()
        {
            MinerStatusCheckTimer = new Timer();
            MinerStatusCheckTimer.Elapsed += MinerStatusCheckTimer_Elapsed;
            MinerStatusCheckTimer.Interval = 30000;  // 30 second default right now.  EWBF won't display any data until it submits the first share.
            MinerStatusCheckTimer.Enabled = true;
        }

        #endregion

        #region Commands

        // Relay commands used for command binding to the view
        public RelayCommand CommandStartMining { get; set; }
        public RelayCommand CommandStopMining { get; set; }

        #endregion

        #region Properties

        // Timer for Monitoring Miner
        private Timer MinerStatusCheckTimer;
        private Timer MinerStatusInsertTimer;
        private MiningSession MiningSession = new MiningSession();
        private bool MiningStatsStarted = false;

        public MainWindow mainWindow;

        // Main Title string used to bind to UI
        private string labelMainTitle;
        public string LabelMainTitle
        {
            get
            {
                if (Application.Current.Properties["WorkerName"] != null)
                    return string.Format("BITPOOL MINER - {0}", Application.Current.Properties["WorkerName"].ToString());
                else
                    return "BITPOOL MINER - OFFLINE";
            }
            set
            {
                if (labelMainTitle == value)
                    return;

                if (Application.Current.Properties["WorkerName"] != null)
                    labelMainTitle = string.Format("BITPOOL MINER - {0}", Application.Current.Properties["WorkerName"].ToString());
                else
                    labelMainTitle = "BITPOOL MINER - OFFLINE";

                // Set title on main window
                if (mainWindow.LabelMainTitle != null)
                    mainWindow.LabelMainTitle.Content = labelMainTitle;

                // Change value and notify UI
                OnPropertyChanged();
            }
        }

        // Account workers property to bind to UI
        private ObservableCollection<AccountWorkers> accountWorkersList;
        public ObservableCollection<AccountWorkers> AccountWorkersList
        {
            get
            {
                InitAccountWorkerList();
                return accountWorkersList;
            }
            set
            {
                accountWorkersList = value;
                OnPropertyChanged();
            }
        }

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

        #endregion

        #region Mining

        /// <summary>
        /// Monitoring timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MinerStatusInsertTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Call miner RPC and post results to API
            MiningSession.GetMinerStatsAsync();
        }

        /// <summary>
        /// Monitoring timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MinerStatusCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Call miner RPC and post results to API
            GetMinerMonitoringResults();
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

                    foreach (MinerMonitorStat minerMonitorStat in minerMonitorStatListGrouped)
                    {
                        minerMonitorStat.DisplayHashRate = HashrateFormatter.Format(minerMonitorStat.CoinType, minerMonitorStat.HashRate);
                    }

                    MinerMonitorStatListGrouped = new ObservableCollection<MinerMonitorStat>(minerMonitorStatListGrouped);

                    // Notify UI of change
                    OnPropertyChanged("MinerMonitorStatListGrouped");
                }
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading monitor data"));
            }
        }

        /// <summary>
        /// Handler to create multiple mining sessions if needed
        /// </summary>
        private void SetupLocalMiners()
        {
            // This logic will be moved to a class to setup the local miners and add them to the mining session
            // This will be done via config file and API calls necessary based on the coins being mined, etc.
            MiningSession.RemoveAllMiners();


            // Call API and retrieve a list of miner configurations used to start mining
            List<MinerConfigResponse> minerConfigResponseList = GetMinerConfigurations();

            // Iterate through returned responses from API and initialize miners
            foreach (MinerConfigResponse minerConfigResponse in minerConfigResponseList)
            {
                // Create miner session
                Miner miner = MinerFactory.CreateMiner(minerConfigResponse.MinerBaseType, minerConfigResponse.HardwareType);
                miner.CoinType = minerConfigResponse.CoinSelectedForMining;
                miner.MinerArguments = minerConfigResponse.MinerConfigString;
                MiningSession.AddMiner(miner);
                ShowInformation(string.Format("Mining started {0} {1}", minerConfigResponse.MinerBaseType, minerConfigResponseList[0].MinerConfigString));
            }
        }

        /// <summary>
        /// Call API and retrieve a list of miner configurations used to start mining
        /// </summary>
        /// <returns></returns>
        private static List<MinerConfigResponse> GetMinerConfigurations()
        {
            // Build the Request to call the API and retrieve the miner config strings
            List<AccountWallet> accountWalletList = new List<AccountWallet>();
            ObservableCollection<GPUSettings> gpuSettingsList = new ObservableCollection<GPUSettings>();
            Region region = Region.UNDEFINED;

            // Get configurations needed for building API request from Application settings
            accountWalletList = (List<AccountWallet>)Application.Current.Properties["AccountWalletList"];
            gpuSettingsList = (ObservableCollection<GPUSettings>)Application.Current.Properties["GPUSettingsList"];
            region = (Region)Application.Current.Properties["Region"];

            // Build actual Request object
            MinerConfigRequest minerConfigRequest = new MinerConfigRequest();
            minerConfigRequest.Region = region;
            minerConfigRequest.AccountWalletList = accountWalletList;
            minerConfigRequest.GPUSettingsList = gpuSettingsList.ToList();

            // Call the web API the get a response with a list of miner config strings used
            // to start one or more mining sessions based on the current miners configurations
            MinerConfigStringAPI minerConfigStringAPI = new MinerConfigStringAPI();
            List<MinerConfigResponse> minerConfigResponseList = minerConfigStringAPI.GetMinerConfigResponses(minerConfigRequest);
            return minerConfigResponseList;
        }

        /// <summary>
        /// Main entry point to start mining
        /// Wires up multiple sessions and starts mining events
        /// </summary>
        /// <param name="parameters"></param>
        private void StartAllMiners(object parameters)
        {
            SetupLocalMiners();
            MiningSession.StartMiningSession();
            MinerStatusCheckTimer.Enabled = true;
            MiningStatsStarted = true;
        }

        /// <summary>
        /// Main entry point to stop mining
        /// Ends all mining sessions
        /// </summary>
        /// <param name="parameters"></param>
        private void StopAllMiners(object parameters)
        {
            MiningStatsStarted = false;
            MinerStatusCheckTimer.Enabled = false;
            MiningSession.StopMiningSession();
        }

        #endregion

        #region Main Window Menu Worker List Methods

        private void InitAccountWorkerList()
        {
            // If there is an account id set, and there are no records in the list then load from API
            if (Application.Current.Properties["AccountID"] != null && accountWorkersList.Count == 0)
            {
                // Load list of account workers
                AccountWorkersAPI accountWorkersAPI = new AccountWorkersAPI();
                accountWorkersList = accountWorkersAPI.GetAccountWorkers();
            }
        }

        public void AddAccountWorker(Guid AccountGuid, string WorkerName)
        {

            AccountWorkers accountWorkers = new AccountWorkers();
            accountWorkers.AccountGuid = AccountGuid;
            accountWorkers.WorkerName = WorkerName;
            AccountWorkersList.Add(accountWorkers);
            // Notify UI of change
            OnPropertyChanged("AccountWorkersList");
        }

        public void RemoveAccountWorker(AccountWorkers accountWorkers)
        {
            AccountWorkersList.Remove(accountWorkers);
            // Notify UI of change
            OnPropertyChanged("AccountWorkersList");
        }

        #endregion

    }
}
