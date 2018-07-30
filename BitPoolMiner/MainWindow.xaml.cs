using BitPoolMiner.Utils;
using BitPoolMiner.Utils.CommandConverter;
using BitPoolMiner.Utils.FeatureTour;
using BitPoolMiner.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using ThinkSharp.FeatureTouring.Navigation;

namespace BitPoolMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // NLog
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // ViewModels 
        private GettingStartedViewModel GettingStartedViewModel;
        private WalletViewModel WalletViewModel;
        private AccountViewModel AccountViewModel;
        private MainWindowViewModel MainWindowViewModel;
        private MonitorViewModel MonitorViewModel;
        private WorkerViewModel WorkerViewModel;
        private ProfitabilityViewModel ProfitabilityViewModel;

        #region Init

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // Initialize ViewModels
                if (MainWindowViewModel == null)
                    MainWindowViewModel = (MainWindowViewModel)this.DataContext;

                if (AccountViewModel == null)
                    AccountViewModel = new AccountViewModel(MainWindowViewModel);

                if (GettingStartedViewModel == null)
                    GettingStartedViewModel = new GettingStartedViewModel();

                if (WalletViewModel == null)
                    WalletViewModel = new WalletViewModel();

                if (MonitorViewModel == null)
                    MonitorViewModel = new MonitorViewModel();

                if (WorkerViewModel == null)
                    WorkerViewModel = new WorkerViewModel();

                DataContext = MonitorViewModel;

                // Display MainView data
                MainWindowViewModel.GetMinerMonitoringResults();
                MainWindowViewModel.InitWhatToMine();
                MainWindowViewModel.InitPayments();

                //Force window size to prevent crashing
                ResizeWindow();

                // Initialize Profitability ViewModel after main window data loaded
                if (ProfitabilityViewModel == null)
                    ProfitabilityViewModel = new ProfitabilityViewModel(MainWindowViewModel);

                // Pass a reference of the Profitability ViewMode to the MainWindowViewModel.  This will allow 
                // rerendering of charts from the main window view timer event
                MainWindowViewModel.profitabilityViewModel = ProfitabilityViewModel;

                // Pass a reference of the Wallet View Model to the Account View model.  
                // This will allow the account view model to refresh the wallet if needed when linking account id's.
                AccountViewModel.WalletViewModel = WalletViewModel;

                // Check to see if should start mining on applications tart.
                CheckAutoStartMining();

                // Handle special Feature Tour Navigation events
                InitFeatureTourNavigation();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unhandled exception caught");
            }
        }
        #endregion

        #region Feature Tour Navigation 

        /// <summary>
        /// Handle special Feature Tour Navigation events
        /// </summary>
        private void InitFeatureTourNavigation()
        {
            var navigator = FeatureTour.GetNavigator();

            navigator.OnStepEntering(FeatureTourElementID.AccountViewButtonSaveHardwareSettings).Execute(s => DataContext = AccountViewModel);
            navigator.OnStepEntering(FeatureTourElementID.AccountViewTextBoxAccountID).Execute(s => DataContext = AccountViewModel);
            navigator.OnStepEntering(FeatureTourElementID.AccountViewTextBoxWorkerName).Execute(s => DataContext = AccountViewModel);
            navigator.OnStepEntering(FeatureTourElementID.WalletViewDataGridWalletAddresses).Execute(s => DataContext = WalletViewModel);
        }

        #endregion

        #region Auto Start Mining
        private void CheckAutoStartMining()
        {
            try
            {
                if ((bool)Application.Current.Properties["AutoStartMining"])
                {
                    ButtonStartMining_Click(this, new RoutedEventArgs());
                }
            }
            catch
            {
                // if the value isn't set, just ignore it and assume false
            }
        }

        #endregion

        #region Menu Button Click Events

        private void MinerButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = AccountViewModel;
        }

        private void GettingStartedButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = GettingStartedViewModel;
        }

        private void WalletSettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = WalletViewModel;
        }

        private void WorkerButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            WorkerParameters workerParameters = (WorkerParameters)button.CommandParameter;

            WorkerViewModel.WorkerName = workerParameters.WorkerName;
            WorkerViewModel.CoinType = workerParameters.CoinType;

            WorkerViewModel.InitMonitorMining();
            WorkerViewModel.InitMonitorMining24Hour();
            WorkerViewModel.InitCoinMarketCap();
            DataContext = WorkerViewModel;
        }

        private void AccountSettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = AccountViewModel;
        }

        private void ProfitabilityButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = ProfitabilityViewModel;
        }

        private void MiningDashboardButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = MonitorViewModel;
        }

        // Navigate to website
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #endregion

        #region Mining Button Click Events

        /// <summary>
        /// Stop mining and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel.CommandStopMining.Execute(null);
            NLogProcessing.StopLoggingMainWindowClose();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Start mining button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartMining_Click(object sender, RoutedEventArgs e)
        {
            ButtonStartMining.IsEnabled = false;
            ButtonStopMining.IsEnabled = true;

            MainWindowViewModel.CommandStartMining.Execute(null);
        }

        /// <summary>
        /// Stop mining button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStopMining_Click(object sender, RoutedEventArgs e)
        {
            ButtonStartMining.IsEnabled = true;
            ButtonStopMining.IsEnabled = false;
            MainWindowViewModel.CommandStopMining.Execute(null);
        }

        #endregion

        #region Window Resizing

        private bool _inStateChange;

        /// <summary>
        /// Force window size to prevent crashing
        /// </summary>
        private void ResizeWindow()
        {
            if (WindowState != WindowState.Minimized && !_inStateChange)
            {
                _inStateChange = true;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanMinimize;
                Height = SystemParameters.PrimaryScreenHeight - 100;
                Width = SystemParameters.PrimaryScreenWidth - 100;
                _inStateChange = false;
            }
        }

        /// <summary>
        /// Force the app to be full sized
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            try
            {
                ResizeWindow();
                base.OnStateChanged(e);
            }
            catch
            {
                // eat it
            }
        }

        #endregion
    }
}
