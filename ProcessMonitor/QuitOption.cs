
namespace ProcessMonitor
{
    public class QuitOption
    {
        public void ReadKey()
        {
            Console.WriteLine("Press Q to exit console application.\n");
            ConsoleKeyInfo currentInputKey = Console.ReadKey();
            while (true)
            {
                if (!currentInputKey.Equals(null) && currentInputKey.Key == ConsoleKey.Q)
                {
                    
                    Environment.Exit(0);
                }
            }
        }
    }
}
        

