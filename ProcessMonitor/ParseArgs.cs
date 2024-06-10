using CommandLine;

    public class ParseArgs
    {
        [Option('p', "process", Required = true, HelpText = "Set the process of interest.")]
        public string Process { get; set; }
        [Option('l', "lifetime", Required = true, HelpText = "Set maximum lifetime of the process of interest.")]
        public int Lifetime { get; set; }
        [Option('f', "frequency", Required = true, HelpText = "Set the monitoring frequency in minutes.")]
        public int Frequency { get; set; }

    }

