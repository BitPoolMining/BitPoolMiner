using BitPoolMiner.Enums;
using BitPoolMiner.Miners;
using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;


namespace BitPoolMiner.ViewModels
{
    /// <summary>
    /// View model for main window
    /// Monitoring logic moved to partial class
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
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

        #endregion

        #region Properties

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
