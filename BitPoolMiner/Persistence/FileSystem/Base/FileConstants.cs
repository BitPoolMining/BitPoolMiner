using BitPoolMiner.Utils;
using System.IO;

namespace BitPoolMiner.Persistence.FileSystem.Base
{
    /// <summary>
    /// Constants for Config files
    /// </summary>
    public static class FileConstants
    {
        // Use miners folder until we figure out a more appropriate location
        public const string BPMFolderName = "BPM";
        public const string ConfigFolderName = "Config";
        public const string MinersFolderName = "MinerApps";
        public const string LogFolderName = "LogFiles";
        public const string LogFileName = "BPMLog.txt";

        public static string ConfigFilePath()
        {
            var dir = Path.Combine(Core.GetUserConfigBaseDirectory(), ConfigFolderName);
            Core.ValidateDirectory(dir);
            return dir;
        }

        public static string LogFilePath()
        {
            var dir = Path.Combine(Utils.Core.GetLogFileBaseDirectory(), LogFolderName);
            Core.ValidateDirectory(dir);
            return dir;
        }
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
