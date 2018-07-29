using BitPoolMiner.Persistence.FileSystem.Base;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitPoolMiner.Utils
{
    public class NLogProcessing
    {
        //#region "NLog"
        //NLog.Config.LoggingConfiguration config;
        //NLog.Targets.FileTarget logfile;
        //#endregion

        public static void SetupLogging()
        {
            try
            {

                var logFilePath = FileConstants.LogFilePath();
                var logFile = Path.Combine(logFilePath, FileConstants.LogFileName);

                NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
                NLog.Targets.FileTarget logfile = new NLog.Targets.FileTarget()
                {
                    FileName = logFile,
                    Layout = "${longdate} ${level} ${callsite}: ${message}  ${exception}",
                    ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.DateAndSequence,
                    MaxArchiveFiles = 3,
                    ArchiveAboveSize = 1048576  // 1 MB
                };

                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
                NLog.LogManager.Configuration = config;
            }
            catch
            {
                // Could not create log folder for some reason.
                // Let this continue for now.
            }
        }

        public static void StartLogging()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Application launched - logging initialized");
        }

        public static void StopLoggingMainWindowClose()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Application Shutting Down due to main window close");

            NLog.LogManager.Shutdown();
        }

        public static void LogInfo(string message)
        {
            try
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info(message);
            }
            catch
            {
                // If cannot log then don't blow up
            }
        }

        public static void LogError(Exception e, string message)
        {
            try
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e, message);
            }
            catch
            {
                // If cannot log then don't blow up
            }
        }

    }
}
