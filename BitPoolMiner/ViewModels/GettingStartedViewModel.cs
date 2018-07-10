using BitPoolMiner.Utils.FeatureTour.Tours;
using BitPoolMiner.ViewModels.Base;
using System;

namespace BitPoolMiner.ViewModels
{
    public class GettingStartedViewModel : ViewModelBase
    {
        #region Commands

        public RelayCommand CmdStartTourFirstMiningWorkerSetup { get; set; }

        #endregion

        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public GettingStartedViewModel()
        {
            // Wire up commands
            CmdStartTourFirstMiningWorkerSetup = new RelayCommand(StartTourFirstMiningWorkerSetup);
        }

        #endregion

        #region Tours

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

        #endregion
    }
}
