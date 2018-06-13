using BitPoolMiner.ViewModels;
using System.Windows;

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

            InitializeComponent();

            DataContext = MonitorViewModel;

            // Display MainView data
            MainWindowViewModel.GetMinerMonitoringResults();
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
    }
}
