using System;

namespace motInboundLib
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Please, specify directory an ip address to use via commandline arguments.");
                return;
            }

            new fileSystemWatcher(args[0], args[1]);
        }
    }
}