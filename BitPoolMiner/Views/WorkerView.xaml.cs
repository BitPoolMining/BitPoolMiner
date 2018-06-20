using LiveCharts.Wpf;
using System.Windows.Controls;

namespace BitPoolMiner.Views
{
    /// <summary>
    /// Interaction logic for WorkerView.xaml
    /// </summary>
    public partial class WorkerView : UserControl
    {
        public WorkerView()
        {
            InitializeComponent();

            //ChartMinerMonitorStatsHashRate.AxisY.Add(new Axis
            //{
            //    LabelFormatter = value => value.ToString(),
            //    Title = "Hashrate"
            //});

            //ChartMinerMonitorStatsPower.AxisY.Add(new Axis
            //{
            //    LabelFormatter = value => value.ToString(),
            //    Title = "Power"
            //});
        }
    }
}
