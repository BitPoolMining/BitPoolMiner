using System.IO;

namespace BitPoolMiner.Persistence.FileSystem.Base
{
    /// <summary>
    /// Constants for Config files
    /// </summary>
    public static class FileConstants
    {
        // Use miners folder until we figure out a more appropriate location
        public const string ConfigFolderName = "Config";
        public const string MinersFolderName = "miners";

        public static string ConfigFilePath = Path.Combine(Utils.Core.GetBaseBPMBaseDir(), ConfigFolderName);
    }

    /// <summary>
    /// Name of JSON config files
    /// </summary>
    public static class FileNameConstants
    {
        /// <summary>
        /// Config file used to store the account identity
        /// </summary>
        public const string AccountIdentityFileName = "AccountIdentity.json";

        /// <summary>
        /// Config file used to store the worker settings
        /// </summary>
        public const string WorkerSettingsFileName = "WorkerSettings.json";
    }
}
