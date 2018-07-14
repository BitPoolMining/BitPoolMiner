using BitPoolMiner.Utils.FeatureTour.Tours;
using BitPoolMiner.ViewModels.Base;
using System;

namespace BitPoolMiner.ViewModels
{
    public class GettingStartedViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand CmdStartTourFirstMiningWorkerSetup { get; set; }
        public RelayCommand CmdStartTourMonitoringOnlyInstance { get; set; }
        public RelayCommand CmdStartTourMineMultipleCoins { get; set; }
        public RelayCommand CmdStartTourAdditionalWorkerSetup { get; set; }

        #endregion

        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public GettingStartedViewModel()
        {
            // Wire up commands
            CmdStartTourFirstMiningWorkerSetup = new RelayCommand(StartTourFirstMiningWorkerSetup);
            CmdStartTourMonitoringOnlyInstance = new RelayCommand(StartTourMonitoringOnlyInstance);
            CmdStartTourMineMultipleCoins = new RelayCommand(StartTourMineMultipleCoins);
            CmdStartTourAdditionalWorkerSetup = new RelayCommand(StartTourAdditionalWorkerSetup);
        }

        #endregion

        #region Tours

        /// <summary>
        /// Set up first worker
        /// </summary>
        /// <param name="parameter"></param>
        public void StartTourFirstMiningWorkerSetup(object parameter)
        {
            try
            {
                TourFirstMiningWorkerSetup.StartTourFirstMiningWorkerSetup();
            }
            catch (Exception e)
            {
                ShowError("Error starting tour");
            }
        }

        /// <summary>
        /// Set up monitor only instance
        /// </summary>
        /// <param name="parameter"></param>
        public void StartTourMonitoringOnlyInstance(object parameter)
        {
            try
            {
                TourMonitoringOnlyInstance.StartTourMonitoringOnlyInstance();
            }
            catch (Exception e)
            {
                ShowError("Error starting tour");
            }
        }

        /// <summary>
        /// Set up mine multiple coins
        /// </summary>
        /// <param name="parameter"></param>
        public void StartTourMineMultipleCoins(object parameter)
        {
            try
            {
                TourMineMultipleCoins.StartTourMineMultipleCoins();
            }
            catch (Exception e)
            {
                ShowError("Error starting tour");
            }
        }

        /// <summary>
        /// Set up additional worker setup
        /// </summary>
        /// <param name="parameter"></param>
        public void StartTourAdditionalWorkerSetup(object parameter)
        {
            try
            {
                TourAdditionalWorkerSetup.StartTourAdditionalWorkerSetup();
            }
            catch (Exception e)
            {
                ShowError("Error starting tour");
            }
        }

        #endregion
    }
}
