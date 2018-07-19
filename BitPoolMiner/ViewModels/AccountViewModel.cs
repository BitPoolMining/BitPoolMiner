using BitPoolMiner.Enums;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.Persistence.FileSystem;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BitPoolMiner.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainWindowViewModel;

        #region Properties

        // Account GUID property used for data binding to the view
        private AccountIdentity AccountIdentity;
        public Guid AccountGuid
        {
            get
            {
                return AccountIdentity.AccountGuid;
            }
            set
            {
                if (AccountIdentity.AccountGuid == value)
                    return;

                // Change value and notify UI
                AccountIdentity.AccountGuid = value;
                OnPropertyChanged();

                // Set global variable for Account ID
                Application.Current.Properties["AccountID"] = AccountIdentity.AccountGuid;
            }
        }

        // Account workers property to bind to UI
        private ObservableCollection<AccountWorkers> accountWorkersList;
        public ObservableCollection<AccountWorkers> AccountWorkersList
        {
            get
            {
                return accountWorkersList;
            }
            set
            {
                accountWorkersList = value;
                OnPropertyChanged();
            }
        }

        // Worker settings property to bind to UI
        private WorkerSettings workerSettings;
        public WorkerSettings WorkerSettings
        {
            get
            {
                return workerSettings;
            }
            set
            {
                if (workerSettings == value)
                    return;

                // Change value and notify UI
                workerSettings = value;
                OnPropertyChanged();

                // Set global variable for Worker Name, Region and Currency
                Application.Current.Properties["WorkerName"] = WorkerSettings.WorkerName;
                Application.Current.Properties["Region"] = WorkerSettings.Region;
                Application.Current.Properties["Currency"] = WorkerSettings.Currency;

                // Set Label on Main Window
                _mainWindowViewModel.LabelMainTitle = string.Format("BITPOOL MINER - {0}", WorkerSettings.WorkerName);

                // Save settings to config file
                PersistWorkerSettings(null);
            }
        }

        // List of GPU hardware settings
        private ObservableCollection<GPUSettings> gpuSettingsList;
        public ObservableCollection<GPUSettings> GPUSettingsList
        {
            get { return gpuSettingsList; }
            set
            {
                gpuSettingsList = value;

                OnPropertyChanged();

                if (value == null)
                    return;

                // Set global variable for Worker Name
                Application.Current.Properties["GPUSettingsList"] = GPUSettingsList;
            }
        }

        // WalletViewModel reference
        public WalletViewModel WalletViewModel { get; set; }


        #endregion

        #region Commands

        // Relay commands used for command binding to the view
        public RelayCommand CommandGetNewAccountID { get; set; }
        public RelayCommand CommandUpdateAccountID { get; set; }
        public RelayCommand CommandRemoveWorker { get; set; }
        public RelayCommand CommandSaveWorkerSettings { get; set; }
        public RelayCommand CommandScanHardware { get; set; }
        public RelayCommand CommandSaveAccountWorkerHardware { get; set; }
        public RelayCommand CommandUpdateCoinType { get; set; }

        #endregion

        #region Init

        /// <summary>
        /// Constrtuctor
        /// </summary>
        public AccountViewModel(MainWindowViewModel mainWindowViewModel)
        {
            // Get a reference back to the main window view model
            _mainWindowViewModel = mainWindowViewModel;

            // Instatiate new Account Identity
            AccountIdentity = new AccountIdentity();

            // Wire up commands for Account Identity
            CommandGetNewAccountID = new RelayCommand(GetNewAccountID);
            CommandUpdateAccountID = new RelayCommand(UpdateAccountID);
            CommandRemoveWorker = new RelayCommand(RemoveAccountWorkers);

            // Wire up commands for button clicks
            CommandSaveWorkerSettings = new RelayCommand(PersistWorkerSettings);
            CommandScanHardware = new RelayCommand(ScanHardware);
            CommandSaveAccountWorkerHardware = new RelayCommand(PersistWorkerHardware);
            CommandUpdateCoinType = new RelayCommand(UpdateCoinType);

            // Load previous GUID or get a new GUID
            InitAccountID();

            // Load list of workers for account
            InitAccountWorkers();

            // Load worker settings from config file or set the default worker settings and save to file
            InitWorkerSettings();

            // Load hardware settings from API or scan for hardware
            InitWorkerHardware();

            // Update worker list on main window
            _mainWindowViewModel.GetAccountWorkerList();


        }

        /// <summary>
        /// Load previous GUID from config file or get a new GUID from the API
        /// </summary>
        private void InitAccountID()
        {
            try
            {
                // Empty GUID used for validation
                Guid guidEmpty = new Guid();

                // Attempt to read the GUID from the config file
                AccountIdentityFile accountIdentityFile = new AccountIdentityFile();
                AccountIdentity = accountIdentityFile.ReadJsonFromFile();

                // Validate if a GUID was actually read
                if (AccountIdentity.AccountGuid == guidEmpty)
                {
                    try
                    {
                        // If no GUID found, then get a new GUID from the API
                        AccountIdentityAPI accountIdentityAPI = new AccountIdentityAPI();
                        AccountIdentity = accountIdentityAPI.GetAccountID();

                        // Notify UI to update
                        OnPropertyChanged("AccountGuid");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("There was an error retrieving a new account id from the web. Please create a new one manually by selecting 'new account' before mining.");
                    }
                    // Write GUID to account identity config file
                    accountIdentityFile.WriteJsonToFile(AccountIdentity);

                    // Insert new worker for account
                    InsertAccountWorkers();
                }

                // Set global variable for Account ID
                Application.Current.Properties["AccountID"] = AccountIdentity.AccountGuid;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error initializing account id"), e);
            }
        }

        /// <summary>
        /// Load list of workers for account
        /// </summary>
        private void InitAccountWorkers()
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null)
                {
                    // Load list of account workers
                    AccountWorkersAPI accountWorkersAPI = new AccountWorkersAPI();
                    AccountWorkersList = accountWorkersAPI.GetAccountWorkers();

                    // Notify UI of change
                    OnPropertyChanged("AccountWorkersList");
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error loading worker list"), e);
            }
        }

        private void InitWorkerSettings()
        {
            try
            {
                // Attempt to read the worker settings from the config file
                WorkerSettingsFile workerSettingsFile = new WorkerSettingsFile();
                WorkerSettings = workerSettingsFile.ReadJsonFromFile();

                // Set global variable for Worker Name, Region and Currency
                Application.Current.Properties["WorkerName"] = WorkerSettings.WorkerName;
                Application.Current.Properties["Region"] = WorkerSettings.Region;
                Application.Current.Properties["Currency"] = WorkerSettings.Currency;
                Application.Current.Properties["AutoStartMining"] = WorkerSettings.AutoStartMining;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error getting worker settings", e));
            }
        }

        private void InitWorkerHardware()
        {
            // Get worker hardware from API
            ReadWorkerHardwareFromAPI();

            if (GPUSettingsList.Count == 0)
            {
                // If no results from API then scan hardware and persist to API
                ScanHardware(null);
            }

            // Set global variable for Worker Name
            Application.Current.Properties["GPUSettingsList"] = GPUSettingsList;
        }
               

        #endregion

        #region AccountIdentity - Unique Account Guid Handling

        /// <summary>
        /// Call API and retrieve new GUID
        /// </summary>
        /// <param name="parameter"></param>
        public void GetNewAccountID(object parameter)
        {
            try
            {
                // Call API to get new GUID
                // Set GUID in account identity object
                AccountIdentityAPI accountIdentityAPI = new AccountIdentityAPI();
                AccountIdentity = accountIdentityAPI.GetAccountID();

                // Set global variable for Account ID
                Application.Current.Properties["AccountID"] = AccountIdentity.AccountGuid;

                // Write GUID to account identity config file
                AccountIdentityFile accountIdentityFile = new AccountIdentityFile();
                accountIdentityFile.WriteJsonToFile(AccountIdentity);

                // Notify UI to update
                OnPropertyChanged("AccountGuid");

                // Removed previous collection of account workers
                accountWorkersList = new ObservableCollection<AccountWorkers>();

                // Insert new worker for account
                InsertAccountWorkers();

                // Notify success
                ShowSuccess("New Account ID created");
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error retrieving new account id"), e);
            }
        }

        /// <summary>
        /// Update GUID when user enters new value in UI and set data in config file
        /// </summary>
        /// <param name="parameter"></param>
        public void UpdateAccountID(object parameter)
        {
            try
            {
                if (parameter == null)
                    return;

                // Set GUID in account identity object
                Guid parsedGuid;
                bool parseResult = Guid.TryParse(parameter.ToString(), out parsedGuid);
                if (parseResult == false)
                    throw new ApplicationException(string.Format("Error {0} is not a valid account id", parsedGuid));

                // Set global variable for Account ID
                AccountIdentity.AccountGuid = parsedGuid;
                Application.Current.Properties["AccountID"] = AccountIdentity.AccountGuid;

                // Write GUID to account identity config file
                AccountIdentityFile accountIdentityFile = new AccountIdentityFile();
                accountIdentityFile.WriteJsonToFile(AccountIdentity);

                // Notify UI to update
                OnPropertyChanged("AccountGuid");

                // Insert new worker for account
                InsertAccountWorkers();

                // Reload Wallet Addresses
                WalletViewModel.CommandInitAccountWallet.Execute(null);

                // Notify success
                ShowSuccess("Updated existing Account ID");
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error updating account id"), e);
            }
        }

        #endregion

        #region Account Workers - List or remove workers from an account

        /// <summary>
        /// Insert a new worker for account
        /// </summary>
        private void InsertAccountWorkers()
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null && WorkerSettings.WorkerName != null)
                {
                    // Reload Account Work List
                    InitAccountWorkers();

                    // Create new worker object
                    AccountWorkers accountWorker = new AccountWorkers();
                    accountWorker.AccountGuid = (Guid)Application.Current.Properties["AccountID"];
                    accountWorker.WorkerName = WorkerSettings.WorkerName;

                    // Insert new worker for account into API
                    AccountWorkersAPI accountWorkersAPI = new AccountWorkersAPI();
                    accountWorkersAPI.PostAccountWorkers(accountWorker);

                    // Insert worker into local list if it doesnt already exist
                    List<AccountWorkers> AccountWorkersListMatches = AccountWorkersList.Where(x => x.AccountGuid == accountWorker.AccountGuid && x.WorkerName == accountWorker.WorkerName).ToList();
                    if (AccountWorkersListMatches.Count == 0)
                    {
                        // Add new worker to local list
                        AccountWorkersList.Add(accountWorker);
                    }

                    // Update worker list on main window
                    _mainWindowViewModel.GetAccountWorkerList();

                    // Set Label on Main Window
                    _mainWindowViewModel.LabelMainTitle = string.Format("BITPOOL MINER - {0}", WorkerSettings.WorkerName);

                    // Notify UI of change
                    OnPropertyChanged("AccountWorkersList");
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error inserting worker to account"), e);
            }
        }

        // Remove worker from list
        private void RemoveAccountWorkers(object param)
        {
            try
            {
                if (param != null)
                {
                    AccountWorkers accountWorkersRemove = (AccountWorkers)param;

                    // Delete worker from account using API
                    AccountWorkersAPI accountWorkersAPI = new AccountWorkersAPI();
                    accountWorkersAPI.DeleteAccountWorkers(accountWorkersRemove);

                    // Remove worker from local list of workers
                    AccountWorkersList.Remove(accountWorkersRemove);

                    // Notify UI of change
                    OnPropertyChanged("AccountWorkersList");

                    // Update worker list on main window
                    _mainWindowViewModel.GetAccountWorkerList();

                    // Notify success
                    ShowSuccess(string.Format("Worker {0} successfully removed", accountWorkersRemove.WorkerName));
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error removing worker from account"), e);
            }
        }

        #endregion

        #region WorkerSettings

        private bool ValidateWorkerSettings(WorkerSettings workerSettingsValidate)
        {
            bool isValid = true;

            // Validate that Workername is set
            if (workerSettingsValidate.WorkerName == null || workerSettingsValidate.WorkerName == "")
            {
                ShowError("Worker name must be set");
                isValid = false;
            }

            // Validate that Workername is not WORKER
            if (workerSettingsValidate.WorkerName == "Worker")
            {
                ShowError("Please use another name other than WORKER");
                isValid = false;
            }

            // Validate that region is set
            if (workerSettingsValidate.Region == Enums.Region.UNDEFINED)
            {
                ShowError("Please select a region");
                isValid = false;
            }

            // Validate that currency is set
            if (workerSettingsValidate.Currency == Enums.CurrencyList.UNDEFINED)
            {
                ShowError("Please select a currency");
                isValid = false;
            }

            return isValid;
        }

        public void PersistWorkerSettings(object param)
        {
            try
            {
                if (ValidateWorkerSettings(WorkerSettings) == false)
                {
                    return;
                }

                // Write GUID to account identity config file
                WorkerSettingsFile workerSettingsFile = new WorkerSettingsFile();
                workerSettingsFile.WriteJsonToFile(WorkerSettings);

                // Notify UI of change
                OnPropertyChanged("WorkerSettings");

                // Set global variable for Worker Name, Region and Currency
                Application.Current.Properties["WorkerName"] = WorkerSettings.WorkerName;
                Application.Current.Properties["Region"] = WorkerSettings.Region;
                Application.Current.Properties["Currency"] = WorkerSettings.Currency;

                // Insert new worker for account if it doesnt already exist
                InsertAccountWorkers();

                // Notify success
                ShowSuccess(string.Format("Worker settings saved"));
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error saving worker settings"), e);
            }
        }

        #endregion

        #region WorkerHardware

        /// <summary>
        /// Get previously saved hardware GPU settings from API
        /// </summary>
        private void ReadWorkerHardwareFromAPI()
        {
            try
            {
                // Write GPU Settings to API
                GPUSettingsAPI gpuSettingsAPI = new GPUSettingsAPI();
                GPUSettingsList = gpuSettingsAPI.GetGPUSettings();
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error saving worker hardware settings"), e);
            }
        }

        /// <summary>
        /// Scan for hardware using Open Harware Monitor 
        /// Write GPU Settings to API
        /// </summary>
        /// <param name="param"></param>
        private void ScanHardware(object param)
        {
            // Scan for hardware using Open Harware Monitor
            Utils.OpenHardwareMonitor.OpenHardwareMonitor openHardwareMonitor = new Utils.OpenHardwareMonitor.OpenHardwareMonitor();
            GPUSettingsList = openHardwareMonitor.ScanHardware();

            // Push GPU settings to the API
            PersistWorkerHardware(null);

            // Notify success
            ShowSuccess(string.Format("Hardware scanned successfully"));
        }

        /// <summary>
        /// Update coin type for a specific GPU
        /// </summary>
        /// <param name="param"></param>
        private void UpdateCoinType(object param)
        {
            OnPropertyChanged("GPUSettingsList");
        }

        /// <summary>
        /// Write GPU Settings to API
        /// </summary>
        /// <param name="param"></param>
        private void PersistWorkerHardware(object param)
        {
            try
            {
                // Write GPU Settings to API
                GPUSettingsAPI gpuSettingsAPI = new GPUSettingsAPI();
                gpuSettingsAPI.PostGPUSettings(GPUSettingsList);

                // Insert new worker for account if it doesnt already exist
                InsertAccountWorkers();

                // Set global variable for Worker Name
                Application.Current.Properties["GPUSettingsList"] = GPUSettingsList;

                // Notify success
                ShowSuccess(string.Format("Hardware changes saved successfully"));
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error saving worker hardware settings"), e);
            }
        }

        #endregion
    }
}
