using BitPoolMiner.Enums;
using BitPoolMiner.Miners;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Timer = System.Timers.Timer;

namespace BitPoolMiner.ViewModels
{
    /// <summary>
    /// Handles all mining operations for main view
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region Init                

        /// <summary>
        /// Initial objects
        /// </summary>
        private void InitMining()
        {
            // Init timer used for monitoring and mining stats
            MinerStatusInsertTimer = new Timer();
            MinerStatusInsertTimer.Elapsed += MinerStatusInsertTimer_Elapsed;
            MinerStatusInsertTimer.Interval = 120000;  // 2 min default right now.  EWBF won't display any data until it submits the first share.
            MinerStatusInsertTimer.Enabled = false;
        }

        #endregion

        #region Commands

        // Relay commands used for command binding to the view
        public RelayCommand CommandStartMining { get; set; }
        public RelayCommand CommandStopMining { get; set; }

        #endregion

        #region Properties

        // Timer for Monitoring Miner
        private Timer MinerStatusInsertTimer;
        private MiningSession MiningSession = new MiningSession();

        // Status of mining
        private bool MiningStatsStarted = false;
        public string MiningStatus
        {
            get
            {
                if (MiningStatsStarted == true)
                {
                    return "online";
                }
                else
                {
                    return "offline";
                }                
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

        private bool ValidateStartMining()
        {
            bool isValid = true;

            // Validate that Account Guid is set
            if (Application.Current.Properties["AccountID"] == null)
            {
                ShowError("Account ID must be set");
                isValid = false;
            }

            // Validate that Workername is set
            if (Application.Current.Properties["WorkerName"] == null)
            {
                ShowError("Worker name must be set");
                isValid = false;
            }

            // Validate GPU Settings list is set
            ObservableCollection<GPUSettings> gpuSettingsList = new ObservableCollection<GPUSettings>();
            gpuSettingsList = (ObservableCollection<GPUSettings>)Application.Current.Properties["GPUSettingsList"];
            if (gpuSettingsList == null || gpuSettingsList.Count == 0)
            {
                ShowError("GPU settings must be set");
                isValid = false;
            }

            // Validate region is set
            Region region = Region.UNDEFINED;
            region = (Region)Application.Current.Properties["Region"];
            if (region == null || region == Region.UNDEFINED)
            {
                ShowError("Region must be set");
                isValid = false;
            }

            // Validate Wallet Settings list is set
            List<AccountWallet> accountWalletList = new List<AccountWallet>();
            accountWalletList = (List<AccountWallet>)Application.Current.Properties["AccountWalletList"];
            if (accountWalletList == null || accountWalletList.Count == 0)
            {
                ShowError("Account Wallet List must be set");
                isValid = false;
            }

            return isValid;
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
            // Validate that all settings are properly set before starting mining
            if (!ValidateStartMining())
                return;

            SetupLocalMiners();
            MiningSession.StartMiningSession();
            MinerStatusInsertTimer.Enabled = true;
            MiningStatsStarted = true;
            OnPropertyChanged("MiningStatus");
        }

        /// <summary>
        /// Main entry point to stop mining
        /// Ends all mining sessions
        /// </summary>
        /// <param name="parameters"></param>
        private void StopAllMiners(object parameters)
        {
            MiningStatsStarted = false;
            MinerStatusInsertTimer.Enabled = false;
            MiningSession.StopMiningSession();
            OnPropertyChanged("MiningStatus");
        }

        #endregion
    }
}
