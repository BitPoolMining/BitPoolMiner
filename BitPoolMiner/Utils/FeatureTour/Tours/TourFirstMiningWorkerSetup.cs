using ThinkSharp.FeatureTouring.Models;

namespace BitPoolMiner.Utils.FeatureTour.Tours
{
    public static class TourFirstMiningWorkerSetup
    {
        public static void StartTourFirstMiningWorkerSetup()
        {
            var tour = new Tour
            {
                Name = "Set up first mining rig",
                ShowNextButtonDefault = true,
                Steps = new[]
                {
                    // Set up worker account
                    new Step(FeatureTourElementID.AccountViewTextBoxWorkerName, "Enter a unique worker name", "Please give your worker a unique name so it is easy to identify."),
                    new Step(FeatureTourElementID.AccountViewCheckBoxAutoStartMining, "Auto Start Mining", "Enable if you would like to start mining as soon as BPM is open."),
                    new Step(FeatureTourElementID.AccountViewComboBoxRegion, "Mining Region", "Select the region to use when connecting to the pool."),
                    new Step(FeatureTourElementID.AccountViewComboBoxFiatCurrency, "Fiat Currency", "Select the fiat currency that you would like to see your figures displayed in."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveWorkerSettings, "Save", "Once you have set your worker settings, click save."),

                    // Verify worker list
                    new Step(FeatureTourElementID.AccountViewDataGridConnectedWorkers, "Verify Connected Workers List", "Please ensure that your worker is listed here, and that there are no additional workers."),

                    // Set up hardware
                    new Step(FeatureTourElementID.AccountViewDataGridHardware, "Configure Hardware", "Enable all cards to mine with, select which coin to mine and miner for each enabled card."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Save", "Once you have set your hardware settings, click save."),

                    // Set up wallet
                    new Step(FeatureTourElementID.WalletViewDataGridWalletAddresses, "Enter Addresses", "Enter addresses for each coin that you plan to mine."),
                    new Step(FeatureTourElementID.WalletViewButtonSaveWalletAddresses, "Save", "Once you have set up your wallet addresses, click save.") 
                }
            };
            tour.Start();
        }
    }
}
