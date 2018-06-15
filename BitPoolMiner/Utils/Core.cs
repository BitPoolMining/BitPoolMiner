using BitPoolMiner.Persistence.FileSystem.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Utils
{
    public static class Core
    {
        /// <summary>
        /// Returns the base directory of the BitPoolMiner executing assembly.
        /// We'll build the sub-directory info for the individual miners off of this.
        /// </summary>
        /// <returns></returns>
        public static string GetBaseBPMBaseDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetBaseMinersDir()
        {
            return Path.Combine(GetBaseBPMBaseDir(), FileConstants.MinersFolderName);
        }

        public static string GetUserConfigBaseDirectory()
        {
            return GetBPMBaseUserDirectory();
        }

        public static string GetLogFileBaseDirectory()
        {
            return GetBPMBaseUserDirectory();
        }

        public static string GetBPMBaseUserDirectory()
        {
            var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FileConstants.BPMFolderName);
            return baseDir;
        }

        public static void ValidateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Could not create directory '{path}' used to store configuration data. Error was: {e.Message}");
            }

        }
    }
}
