using CommandLine;
using ProcessMonitor;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("\n--------------------------------- Process Monitor --------------------------------- \n");

        //Start quit option thread
        Thread exitReadKey = new Thread(new ThreadStart(new QuitOption().ReadKey));
        exitReadKey.Start();

        //Parse args
        string proc = null;
        int lifetime = 0;
        int freq = 0;
        Parser.Default.ParseArguments<ParseArgs>(args).WithParsed(opts =>
           {
                  // Assign parsed values to local variables
                  proc = opts.Process;
                  lifetime = opts.Lifetime;
                  freq = opts.Frequency;

           }).WithNotParsed(HandleParseError);
        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Was not able to parse arguments: " + errs);
            Environment.Exit(-1);
        }

 
        //Start monitoring
        Console.WriteLine("The process " + proc + " will be checked every "
            + freq + " minute(s).\nProcess will be killed if its lifetime exceeds "
            + lifetime + " minute(s).");
        ProcessMonitor.Monitor currentMonitor = new ProcessMonitor.Monitor(proc, lifetime, freq);
        Thread monitorThread = new Thread(new ThreadStart(currentMonitor.StartMonitoring));
        monitorThread.Start();

        Console.WriteLine("\n----------------------------------------------------------------------------------- \n");

        return;
    }
}
