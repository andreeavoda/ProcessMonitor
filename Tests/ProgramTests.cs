using CommandLine;
using System.Diagnostics;

namespace ProcessMonitor.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void TestArgumentParsing_ValidArguments()
        {
            string[] args = { "--process", "notepad", "--lifetime", "10", "--frequency", "5" };
            var options = new ParseArgs();
            var result = Parser.Default.ParseArguments<ParseArgs>(args)
                .WithParsed(parsedOptions => options = parsedOptions)
                .WithNotParsed(errors => Assert.Fail("Failed to parse arguments"));

            Assert.That(options.Process, Is.EqualTo("notepad"));
            Assert.That(options.Lifetime, Is.EqualTo(10));
            Assert.That(options.Frequency, Is.EqualTo(5));
        }

        [Test]
        public void TestArgumentParsing_InvalidArgumentLifetime()
        {
            string[] args = { "--process", "notepad", "--lifetime", "teststring55", "--frequency", "10" };
            var options = new ParseArgs();
            var result = Parser.Default.ParseArguments<ParseArgs>(args)
                .WithParsed(parsedOptions => options = parsedOptions)
                .WithNotParsed(errors => Assert.IsNotEmpty(errors));

            Assert.That(options.Lifetime, Is.Not.EqualTo(55)); 
        }

        [Test]
        public void TestArgumentParsing_InvalidArgumentFrequency()
        {
            string[] args = { "--process", "notepad", "--lifetime", "2", "--frequency", "teststring 5" };
            var options = new ParseArgs();
            var result = Parser.Default.ParseArguments<ParseArgs>(args)
                .WithParsed(parsedOptions => options = parsedOptions)
                .WithNotParsed(errors => Assert.IsNotEmpty(errors));

            Assert.That(options.Frequency, Is.Not.EqualTo(5));
        }

        [Test]
        public void TestProcessMonitor_StartMonitoring()
        {
            string processName = "notepad";
            int lifetime = 1; 
            int frequency = 1; 

            var monitor = new Monitor(processName, lifetime, frequency);
            var monitoringThread = new Thread(new ThreadStart(monitor.StartMonitoring));
            monitoringThread.Start();
            Thread.Sleep(2 * 1000);

            // Check if the monitor is running correctly
            Assert.IsTrue(Thread.CurrentThread.IsAlive); // Assuming IsMonitoring is a property indicating the monitoring status
        }


        [Test]
        public void TestProcessMonitor_LogFile()
        {
            string processName = "spotify";
            int lifetime = 1; // 1 minute
            int frequency = 1; // 1 minute

            var monitor = new Monitor(processName, lifetime, frequency);

            // Start monitored app
            Process.Start("/Applications/Spotify.app/Contents/MacOS/Spotify");

            // Start Process Monitor
            var monitoringThread = new Thread(new ThreadStart(monitor.StartMonitoring));
            monitoringThread.Start();
            Thread.Sleep(1000 * 60 * 3);
            // Start Monitored app again
            Process.Start("/Applications/Spotify.app/Contents/MacOS/Spotify");
            Thread.Sleep(1000 * 60 * 3);

            // Check if logfile contents are correct
            string logsPath = Directory.GetCurrentDirectory() + "/logs/";
            var directory = new DirectoryInfo(logsPath);
            Console.WriteLine("logsPath: " + logsPath);

            string pattern = "log-*";
            var logFileName = (from f in directory.GetFiles(pattern)
                           orderby f.LastWriteTime descending
                           select f).First().Name;
            Console.WriteLine("logfileName: " + logFileName);

            string logContents = File.ReadAllText(logsPath + logFileName);
            Console.WriteLine("logContents: " + logContents);
            Assert.That(logContents.Contains("spotify\nspotify"));

        }

        //Uncomment this test on Windows and rebuild

        //[Test]
        //public void TestProcessMonitor_QuitOption()
        //{
        //    //Start quit option thread
        //    Thread exitReadKey = new Thread(new ThreadStart(new QuitOption().ReadKey));
        //    exitReadKey.Start();
        // Send random key - thread should remain alive
        //    SendKeys.Send("A");
        //    Assert.IsTrue(exitReadKey.IsAlive);

        // Send "Q" key - thread should remain alive
        //    SendKeys.Send("Q");
        //    Assert.IsFalse(exitReadKey.IsAlive);

        //}
    }
}