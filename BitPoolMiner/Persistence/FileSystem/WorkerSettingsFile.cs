using BitPoolMiner.Models;
using BitPoolMiner.Persistence.FileSystem.Base;
using Newtonsoft.Json;
using System;
using System.IO;
using BitPoolMiner.Utils;

namespace BitPoolMiner.Persistence.FileSystem
{
    /// <summary>
    /// Handles configuration file used to store the worker settings
    /// </summary>
    public class WorkerSettingsFile
    {
        /// <summary>
        /// Serialize object to JSON and write/overwrite file
        /// </summary>
        /// <param name="workerSettings"></param>
        public void WriteJsonToFile(WorkerSettings workerSettings)
        {
            string filePath = Path.Combine(FileConstants.ConfigFilePath(), FileNameConstants.WorkerSettingsFileName);

            try
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, workerSettings);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error writing file {0}", filePath), e);
            }
        }

        /// <summary>
        /// Read object from file and deserialize JSON and map to object
        /// </summary>
        /// <returns></returns>
        public WorkerSettings ReadJsonFromFile()
        {
            string filePath = Path.Combine(FileConstants.ConfigFilePath(), FileNameConstants.WorkerSettingsFileName);
            WorkerSettings workerSettings = new WorkerSettings();

            try
            {
                if (File.Exists(filePath))
                {
                    // deserialize JSON directly from a file
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        workerSettings = (WorkerSettings)serializer.Deserialize(file, typeof(WorkerSettings));
                    }
                }
                else
                {
                    workerSettings = InitEmptyWorkerSettings(workerSettings);
                }
                return workerSettings;
            }
            catch (Exception e)
            {
                workerSettings = InitEmptyWorkerSettings(workerSettings);

                NLogProcessing.LogError(e, "Worker settings could not be loaded. Using default values.");

                // Return defaults
                return workerSettings;
            }            
        }

        /// <summary>
        /// Initializes a new WorkerSettings object (only called when one doesn't exist or cannot be read.
        /// </summary>
        /// <param name="workerSettings"></param>
        /// <returns></returns>
        private WorkerSettings InitEmptyWorkerSettings(WorkerSettings workerSettings)
        {
            // Set default values
            workerSettings.WorkerName = "Worker";
            workerSettings.AutoStartMining = false;
            workerSettings.Region = Enums.Region.USEAST;
            workerSettings.Currency = Enums.CurrencyList.USD;

            // Write defaults to disk
            WriteJsonToFile(workerSettings);

            return workerSettings;
        }
    }
}
