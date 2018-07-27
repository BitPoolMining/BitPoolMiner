using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using BitPoolMiner.Enums;

namespace BitPoolMiner.Process
{
    /// <summary>
    /// This class will be used to handle the individual miner processes. A new instance will be created
    /// for each  miner, and can be used to return data on the process or kill it.
    /// </summary>
    public class BPMProcess
    {
        public System.Diagnostics.Process MinerProcess { get; private set; }

        public bool Start(string workingDirectory, string arguments, string filename, bool forAMD, MinerBaseType minerBaseType)
        {
            try
            {
                MinerProcess = new System.Diagnostics.Process();
                MinerProcess.StartInfo.WorkingDirectory = workingDirectory;
                MinerProcess.StartInfo.FileName = Path.Combine(workingDirectory, filename);
                MinerProcess.StartInfo.CreateNoWindow = false;
                MinerProcess.StartInfo.Arguments = arguments;
                MinerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                MinerProcess.StartInfo.UseShellExecute = true;
                if (forAMD)
                {
                    MinerProcess.StartInfo.UseShellExecute = false;
                    if (MinerProcess.StartInfo.EnvironmentVariables.ContainsKey("GPU_FORCE_64BIT_PTR"))
                        MinerProcess.StartInfo.EnvironmentVariables.Remove("GPU_FORCE_64BIT_PTR");

                    if (MinerProcess.StartInfo.EnvironmentVariables.ContainsKey("GPU_MAX_HEAP_SIZE"))
                        MinerProcess.StartInfo.EnvironmentVariables.Remove("GPU_MAX_HEAP_SIZE");

                    if (MinerProcess.StartInfo.EnvironmentVariables.ContainsKey("GPU_USE_SYNC_OBJECTS"))
                        MinerProcess.StartInfo.EnvironmentVariables.Remove("GPU_USE_SYNC_OBJECTS");

                    if (MinerProcess.StartInfo.EnvironmentVariables.ContainsKey("GPU_MAX_ALLOC_PERCENT"))
                        MinerProcess.StartInfo.EnvironmentVariables.Remove("GPU_MAX_ALLOC_PERCENT");

                    if (MinerProcess.StartInfo.EnvironmentVariables.ContainsKey("GPU_SINGLE_ALLOC_PERCENT"))
                        MinerProcess.StartInfo.EnvironmentVariables.Remove("GPU_SINGLE_ALLOC_PERCENT");

                    MinerProcess.StartInfo.EnvironmentVariables.Add("GPU_FORCE_64BIT_PTR", "0");
                    MinerProcess.StartInfo.EnvironmentVariables.Add("GPU_MAX_HEAP_SIZE", "100");
                    MinerProcess.StartInfo.EnvironmentVariables.Add("GPU_USE_SYNC_OBJECTS", "1");
                    MinerProcess.StartInfo.EnvironmentVariables.Add("GPU_MAX_ALLOC_PERCENT", "100");
                    MinerProcess.StartInfo.EnvironmentVariables.Add("GPU_SINGLE_ALLOC_PERCENT", "100");
                }
                MinerProcess.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("Error launching the miner process {0} with working directory {1}", filename, workingDirectory), e);
            }
            var started = MinerProcess.Start();

            if (MinerProcess.HasExited)
            {
                throw new ApplicationException(string.Format("The miner process started but died instantly. Check the arguments: {0}", MinerProcess.StartInfo.Arguments));
            }

            return started;
        }

        public void KillProcess()
        {
            // Try to force kill it anyway
            try
            {
                MinerProcess.Kill();
            }
            catch
            {
                // if it failed to kill the process ignore it for now.
            }
        }

        ~BPMProcess()
        {
            // Since destroying this class, kill any miner process and dispose of it.
            if (MinerProcess != null && !MinerProcess.HasExited)
                KillProcess();

            if (MinerProcess != null)
                MinerProcess.Dispose();
        }
    }
}
