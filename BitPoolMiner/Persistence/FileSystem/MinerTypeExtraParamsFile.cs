using BitPoolMiner.Models;
using BitPoolMiner.Persistence.FileSystem.Base;
using Newtonsoft.Json;
using System;
using System.IO;
using BitPoolMiner.Utils;
using BitPoolMiner.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BitPoolMiner.Persistence.FileSystem
{
    /// <summary>
    /// Handles configuration file used to store the extra parameters for miner apps to use
    /// </summary>
    public class MinerTypeExtraParamsFile
    {
        /// <summary>
        /// Serialize object to JSON and write/overwrite file
        /// </summary>
        /// <param name="accountMinerTypeExtraParamsList"></param>
        public void WriteJsonToFile(ObservableCollection<AccountMinerTypeExtraParams> accountMinerTypeExtraParamsList)
        {
            string filePath = Path.Combine(FileConstants.ConfigFilePath(), FileNameConstants.MinerTypeExtraParamsFileName);

            try
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, accountMinerTypeExtraParamsList);
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
        public ObservableCollection<AccountMinerTypeExtraParams> ReadJsonFromFile()
        {
            string filePath = Path.Combine(FileConstants.ConfigFilePath(), FileNameConstants.MinerTypeExtraParamsFileName);

            // Create new empty list of miner type extra params
            ObservableCollection<AccountMinerTypeExtraParams> accountMinerTypeExtraParamsList = new ObservableCollection<AccountMinerTypeExtraParams>();

            try
            {
                if (File.Exists(filePath))
                {
                    // deserialize JSON directly from a file
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        accountMinerTypeExtraParamsList = (ObservableCollection<AccountMinerTypeExtraParams>)serializer.Deserialize(file, typeof(ObservableCollection<AccountMinerTypeExtraParams>));
                    }
                }
                else
                {
                    accountMinerTypeExtraParamsList = InitEmptyMinerTypeExtraParams();
                }
                return accountMinerTypeExtraParamsList;
            }
            catch (Exception e)
            {
                accountMinerTypeExtraParamsList = InitEmptyMinerTypeExtraParams();

                NLogProcessing.LogError(e, "Miner type extra parameters could not be loaded. Using default values.");

                // Return defaults
                return accountMinerTypeExtraParamsList;
            }
        }

        /// <summary>
        /// Initializes a new AccountMinerTypeExtraParams object (only called when one doesn't exist or cannot be read).
        /// </summary>
        /// <param name="accountMinerTypeExtraParams"></param>
        /// <returns></returns>
        private ObservableCollection<AccountMinerTypeExtraParams> InitEmptyMinerTypeExtraParams()
        {
            // Create new empty list of miner type extra params
            ObservableCollection<AccountMinerTypeExtraParams> accountMinerTypeExtraParamsList = new ObservableCollection<AccountMinerTypeExtraParams>();

            // Set default values
            foreach (MinerBaseType minerBaseType in Enum.GetValues(typeof(MinerBaseType)))
            {
                // Don't add settings for Undefined
                if (minerBaseType != MinerBaseType.UNDEFINED)
                {
                    AccountMinerTypeExtraParams accountMinerTypeExtraParams = new AccountMinerTypeExtraParams();
                    accountMinerTypeExtraParams.MinerBaseTypeString = minerBaseType.ToString();
                    accountMinerTypeExtraParams.ExtraParams = "";
                    accountMinerTypeExtraParamsList.Add(accountMinerTypeExtraParams);
                }
            }
            // Write defaults to disk
            WriteJsonToFile(accountMinerTypeExtraParamsList);

            return accountMinerTypeExtraParamsList;
        }
    }
}
