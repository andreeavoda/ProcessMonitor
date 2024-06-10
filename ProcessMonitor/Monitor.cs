
using System.Diagnostics;


namespace ProcessMonitor
{
    public class Monitor
    {
        static string MonitoredProcess { get; set; }
        static int MaxLifetime { get; set; }
        static int Frequency { get; set; }

        public Monitor(string process, int lifetime, int frequency)
        {
            MonitoredProcess = process;
            MaxLifetime = lifetime;
            Frequency = frequency;
        }

        public void StartMonitoring()
        {
            //Create log file
            string logFileName = "log-" + DateTime.Now.ToString("yyyyMMddHHss");
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", logFileName);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/logs");
            FileStream file = File.Create(logFilePath);
            file.Close();
            //Start monitoring
            int lifetime = 0;
            int tsleep = 1000 * 60 * Frequency;
            while (true)
            {
                if (IsProcessRunning(MonitoredProcess))
                {
                    Console.WriteLine("Its current lifetime is " + lifetime + " minute(s).");
                    if (lifetime == MaxLifetime)
                    {
                        Console.WriteLine("Reached maximum lifetime. Killing process...");
                        try
                        {
                            KillProcess(MonitoredProcess);
                            using (StreamWriter stream = new StreamWriter(logFilePath,true))
                            {
                                stream.WriteLine(MonitoredProcess);
                                stream.Flush();
                            }
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error while killing the process or writing log file: " + e);

                        }
                    }
                    else
                    {
                        lifetime += Frequency;
                    }
                }
                else
                {
                    lifetime = 0;
                }
                Console.WriteLine("Sleeping for " + Frequency + " minute(s).");
                Thread.Sleep(tsleep);
            }       
        }

        private bool IsProcessRunning(string procName)
        {
            if (procName.ToLower().Contains(".exe"))
            {
                procName = procName.Split('.')[0];
            }
            try
            {
                Process[] proc = Process.GetProcessesByName(procName);
                if (proc[0].ProcessName.Length != 0)
                {
                    Console.WriteLine("Process " + proc[0].ProcessName + " is running.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Process " + procName + " is not running at the moment.");
                    return false;
                }
                    
            }
            catch
            {
                Console.WriteLine("Process " + procName + " is not running at the moment.");
                return false;
            }
        }

        private void KillProcess(string procName)
        {
            if (procName.ToLower().Contains(".exe"))
            {
                procName = procName.Split('.')[0];
            }
            foreach (var process in Process.GetProcessesByName(procName))
            {
                process.Kill();
            }
        }

    }
}
        

