using BitPoolMiner.Enums;
using BitPoolMiner.Miners;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
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
            InitMonitoringTimer();
        }

        /// <summary>
        /// Init timer used for monitoring and mining stats
        /// </summary>
        private void InitMonitoringTimer()
        {
            MinerStatusCheckTimer = new Timer();
            MinerStatusCheckTimer.Elapsed += MinerStatusCheckTimer_Elapsed;
            MinerStatusCheckTimer.Interval = 30000;  // 30 second default right now.  EWBF won't display any data until it submits the first share.
            MinerStatusCheckTimer.Enabled = false;
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

        #endregion

        #region Mining

        /// <summary>
        /// Monitoring timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MinerStatusCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Call miner RPC and post results to API
            MiningSession.GetMinerStatsAsync();
        }

        ///// <summary>
        ///// Start individual mining sessions
        ///// </summary>
        //private async void StartMiner()
        //{
        //    if (MiningStatsStarted == false)
        //    {
        //        MiningStatsStarted = true;
        //        while (MiningStatsStarted == true)
        //        {
        //            Miner miner = MinerFactory.CreateMiner(Enums.MinerBaseType.EWBF, Enums.HardwareType.Nvidia);


        //            // Hard-coding the agruments list here.  This will be read by API, probably from within the EWBF class, but this is just temporary.
        //            // Still deciding where to put the API calls.
        //            //miner.Start("--server us-east.hush.bitpoolmining.com --user t1RVsrQUzTZjTsXxCS3fK8Ec9GrhGswkxEn.jprig1 --pass x --port 3032 --templimit 75 --tempunits C --api --fee 0");

        //            EWBF ewbfMiner = (EWBF)miner;

        //            ewbfMiner.ReportStatsAsyc();
        //            System.Threading.Thread.Sleep(5000);
        //        }
        //    }
        //}

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
