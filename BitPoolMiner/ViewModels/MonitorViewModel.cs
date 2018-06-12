using BitPoolMiner.Models;
using BitPoolMiner.Persistence.API;
using BitPoolMiner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Threading;
using System.Windows;
using Timer = System.Timers.Timer;

namespace BitPoolMiner.ViewModels
{
    public class MonitorViewModel : ViewModelBase
    {
        /// <summary>
        /// Constrtuctor
        /// </summary>
        public MonitorViewModel(MainWindowViewModel mainWindowViewModel)
        {
            // Get a reference back to the main window view model
            _mainWindowViewModel = mainWindowViewModel;

            InitMonitorMining();

            InitMonitoringTimer();
        }

        #region Properties

        // Main Window View Model reference
        private MainWindowViewModel _mainWindowViewModel;

        // Timer for Monitoring Miner
        private Timer MinerStatusCheckTimer;

        // Account wallet property to bind to UI
        private ObservableCollection<MinerMonitorStat> minerMonitorStatList;
        public ObservableCollection<MinerMonitorStat> MinerMonitorStatList
        {
            get
            {
                return minerMonitorStatList;
            }
            set
            {
                minerMonitorStatList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Monitoring

        /// <summary>
        /// Init timer used for monitoring and mining stats
        /// </summary>
        private void InitMonitoringTimer()
        {
            MinerStatusCheckTimer = new Timer();
            MinerStatusCheckTimer.Elapsed += MinerStatusCheckTimer_Elapsed;
            MinerStatusCheckTimer.Interval = 30000;  // 30 second default right now.  EWBF won't display any data until it submits the first share.
            MinerStatusCheckTimer.Enabled = true;
        }

        /// <summary>
        /// Monitoring timer tick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MinerStatusCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Call miner RPC and post results to API
            InitMonitorMining();
        }

        /// <summary>
        /// Load list of miner monitor stats
        /// </summary>
        private void InitMonitorMining()
        {
            try
            {
                if (Application.Current.Properties["AccountID"] != null)
                {
                    // Load list of miner monitor stats
                    MinerMonitorStatsAPI minerMonitorStatsAPI = new MinerMonitorStatsAPI();
                    MinerMonitorStatList = minerMonitorStatsAPI.GetMinerMonitorStats();

                    // Convert GPU List into data tables for UI binding
                    foreach(MinerMonitorStat minerMonitorStat in minerMonitorStatList)
                    {
                        minerMonitorStat.DataTableGPUData = ToDataTable(minerMonitorStat.GPUMonitorStatList);
                        minerMonitorStat.DataTableGPUData = GetTransposedTable(minerMonitorStat.DataTableGPUData);

                        // Remove extra columns that were created when mapping to datatable
                        minerMonitorStat.DataTableGPUData.Columns.RemoveAt(0);

                        // Remove extra rows that were created when mapping to datatable
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(8);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(7);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(6);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(5);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(4);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(3);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(2);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(1);
                        minerMonitorStat.DataTableGPUData.Rows.RemoveAt(0);
                    }

                    // Notify UI of change
                    OnPropertyChanged("MinerMonitorStatList");
                }
            }
            catch (Exception e)
            {
                ShowError(string.Format("Error loading monitor data"));
            }
        }

        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private DataTable GetTransposedTable(DataTable dt)
        {
            DataTable newTable = new DataTable();
            newTable.Columns.Add(new DataColumn("0", typeof(string)));
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataRow newRow = newTable.NewRow();
                newRow[0] = dt.Columns[i].ColumnName;
                for (int j = 1; j <= dt.Rows.Count; j++)
                {
                    if (newTable.Columns.Count < dt.Rows.Count + 1)
                        newTable.Columns.Add(new DataColumn(j.ToString(), typeof(string)));
                    newRow[j] = dt.Rows[j - 1][i];
                }
                newTable.Rows.Add(newRow);
            }
            return newTable;
        }
    }

    #endregion
}

