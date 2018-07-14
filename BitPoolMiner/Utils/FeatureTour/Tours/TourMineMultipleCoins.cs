using ThinkSharp.FeatureTouring.Models;

namespace BitPoolMiner.Utils.FeatureTour.Tours
{
    public static class TourMineMultipleCoins
    {
        public static void StartTourMineMultipleCoins()
        {
            var tour = new Tour
            {
                Name = "Mine multiple coins on one rig",
                ShowNextButtonDefault = true,
                Steps = new[]
                {                    
                    // Set up hardware
                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Configure Hardware", "You can select which cards you would like to mine with."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Configure Hardware", "You can select which coin and which miner to use for each coin."),
                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Configure Hardware", "BPM will run separate miners for you depending on the selections you make."),

                    new Step(FeatureTourElementID.AccountViewButtonSaveHardwareSettings, "Save", "Once you have set your hardware settings, click save.")
                }
            };
            tour.Start();
        }
    }
}
