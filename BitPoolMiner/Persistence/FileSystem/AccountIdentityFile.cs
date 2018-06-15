using BitPoolMiner.Models;
using BitPoolMiner.Persistence.FileSystem.Base;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BitPoolMiner.Persistence.FileSystem
{
    /// <summary>
    /// Handles configuration file used to store the account identity GUID
    /// </summary>
    public class AccountIdentityFile
    {
        /// <summary>
        /// Serialize object to JSON and write/overwrite file
        /// </summary>
        /// <param name="accountIdentity"></param>
        public void WriteJsonToFile(AccountIdentity accountIdentity)
        {
            string filePath = Path.Combine(FileConstants.ConfigFilePath(), FileNameConstants.AccountIdentityFileName);

            try
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, accountIdentity);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error reading reading file {0}", filePath), e);
            }
        }

        /// <summary>
        /// Read object from file and deserialize JSON and map to object
        /// </summary>
        /// <returns></returns>
        public AccountIdentity ReadJsonFromFile()
        {
            string filePath = Path.Combine(FileConstants.ConfigFilePath(), FileNameConstants.AccountIdentityFileName);
            AccountIdentity accountIdentity = new AccountIdentity();

            try
            {

                if (File.Exists(filePath))
                {
                    // deserialize JSON directly from a file
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        accountIdentity = (AccountIdentity)serializer.Deserialize(file, typeof(AccountIdentity));
                    }
                }
                //else
                //{
                //    //if (!Directory.Exists(FileConstants.ConfigFilePath))
                //    //    Directory.CreateDirectory(FileConstants.ConfigFilePath);
                //}
                return accountIdentity;
            }
            catch (Exception)
            {
                // TODO handle case where file does not exist
                //throw new ApplicationException(string.Format("Error writing file {0}", filePath));
                return accountIdentity;
            }
        }
    }
}
