using BitPoolMiner.Utils;
using System.Windows;

namespace BitPoolMiner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupLogging();

            // Force app to only user software rendering
            System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            base.OnStartup(e);
        }

        /// <summary>
        /// Setup logging
        /// </summary>
        private void SetupLogging()
        {
            NLogProcessing.SetupLogging();
            NLogProcessing.StartLogging();
        }

        private void BitPoolMiner_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Hotfix to prevent excessive errors
            // TODO - fix this shit
            if (e.Exception.Message.ToLower() == "Cannot set Owner property to a Window that has not been shown previously.".ToLower())
            {                
                e.Handled = true;
                return;
            }

            logger.Error(e.Exception, $"Unhandled exception caught");
            e.Handled = true;
        }
    }

}
