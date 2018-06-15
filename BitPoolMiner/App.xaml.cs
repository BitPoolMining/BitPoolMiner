using BitPoolMiner.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BitPoolMiner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

}

}
