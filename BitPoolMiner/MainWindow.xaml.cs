using BitPoolMiner.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BitPoolMiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ViewModels      
        private WalletViewModel WalletViewModel;
        private AccountViewModel AccountViewModel;
        private MainWindowViewModel MainWindowViewModel;
        private MonitorViewModel MonitorViewModel;
        private WorkerViewModel WorkerViewModel;

        #region Init

        public MainWindow()
        {
            // Initialize ViewModels
            if (MainWindowViewModel == null)
                MainWindowViewModel = new MainWindowViewModel();
            if (AccountViewModel == null)
                AccountViewModel = new AccountViewModel(MainWindowViewModel);

            if (WalletViewModel == null)
                WalletViewModel = new WalletViewModel();

            if (MonitorViewModel == null)
                MonitorViewModel = new MonitorViewModel(MainWindowViewModel);

            if (WorkerViewModel == null)
                WorkerViewModel = new WorkerViewModel();

            InitializeComponent();

            DataContext = MonitorViewModel;

            // Display MainView data
            MainWindowViewModel.GetMinerMonitoringResults();

            //Force window size to prevent crashing
            ResizeWindow();
        }

        #endregion

        #region Menu Button Click Events

        private void MinerButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = AccountViewModel;
        }

        private void GettingStartedButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = AccountViewModel;
        }

        private void WalletSettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = WalletViewModel;
        }

        private void WorkerButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            WorkerViewModel.WorkerName = button.CommandParameter.ToString();
            DataContext = WorkerViewModel;
        }

        private void AccountSettingsButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = AccountViewModel;
        }

        private void AutoProfitSwitchingButton_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = AccountViewModel;
        }

        private void MiningDashboardButton_Clicked(object sender, RoutedEventArgs e)
        {            
            DataContext = MonitorViewModel;
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
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Start mining button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartMining_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.CommandStartMining.Execute(null);
        }

        /// <summary>
        /// Stop mining button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStopMining_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.CommandStopMining.Execute(null);
        }

        #endregion

        #region Main Window Data Updates

        public void InitTimer()
        {

        }

        #endregion

        #region Window Resizing

        private bool _inStateChange;

        /// <summary>
        /// Force window size to prevent crashing
        /// </summary>
        private void ResizeWindow()
        {
            if (!_inStateChange)
            {
                _inStateChange = true;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanMinimize;
                Height = SystemParameters.PrimaryScreenHeight - 100;
                Width = SystemParameters.PrimaryScreenWidth - 100;
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                _inStateChange = false;
            }
        }

        /// <summary>
        /// Force the app to be full sized
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            ResizeWindow();
            base.OnStateChanged(e);
        }

        #endregion
    }
}
