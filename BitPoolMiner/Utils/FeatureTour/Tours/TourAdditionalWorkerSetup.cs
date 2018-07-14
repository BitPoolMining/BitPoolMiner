using ThinkSharp.FeatureTouring.Models;

namespace BitPoolMiner.Utils.FeatureTour.Tours
{
    public static class TourAdditionalWorkerSetup
    {
        public static void StartTourAdditionalWorkerSetup()
        {
            var tour = new Tour
            {
                Name = "Set up an additional mining rig",
                ShowNextButtonDefault = true,
                Steps = new[]
                {
                    // Set up account ID
                    new Step(FeatureTourElementID.AccountViewTextBoxAccountID, "Use existing account ID", "Please copy and paste your account ID from your previous workers.  This will link your workers together."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveAccountID, "Save", "Once you enter your existing account ID, press save."),

                    // Set up worker account
                    new Step(FeatureTourElementID.AccountViewTextBoxWorkerName, "Enter a unique worker name", "Please give your worker a unique name so it is easy to identify."),
                    new Step(FeatureTourElementID.AccountViewCheckBoxAutoStartMining, "Auto Start Mining", "Enable if you would like to start mining as soon as BPM is open."),
                    new Step(FeatureTourElementID.AccountViewComboBoxRegion, "Mining Region", "Select the region to use when connecting to the pool."),
                    new Step(FeatureTourElementID.AccountViewComboBoxFiatCurrency, "Fiat Currency", "Select the fiat currency that you would like to see your figures displayed in."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveWorkerSettings, "Save", "Once you have set your worker settings, click save."),

                    // Verify worker list
                    new Step(FeatureTourElementID.AccountViewDataGridConnectedWorkers, "Verify Connected Workers List", "Please ensure that your your new worker is listed here."),
                    new Step(FeatureTourElementID.AccountViewDataGridConnectedWorkers, "Verify Connected Workers List", "Please ensure that your previous workers are listed."),
                    new Step(FeatureTourElementID.AccountViewDataGridConnectedWorkers, "Verify Connected Workers List", "Please remove any unneeded workers."),

                    // Set up hardware
                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Configure Hardware", "Enable all cards to mine with, select which coin to mine and miner for each enabled card."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Save", "Once you have set your hardware settings, click save."),

                    // Set up wallet
                    new Step(FeatureTourElementID.WalletViewDataGridWalletAddresses, "Verify Addresses", "Your addresses should now be copied from your previous workers, please verify."),
                    new Step(FeatureTourElementID.WalletViewButtonSaveWalletAddresses, "Save", "If you have made any changes to your wallet addresses, click save.")
                }
            };
            tour.Start();
        }
    }
}
