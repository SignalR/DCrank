using System;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && string.Equals(args[0], "agent", StringComparison.OrdinalIgnoreCase))
            {
                var agent = new Agent();
                agent.Run();
            }
            else if (args.Length == 2 && string.Equals(args[0], "worker", StringComparison.OrdinalIgnoreCase))
            {
                var worker = new Worker(Convert.ToInt32(args[1]));
                worker.Run().Wait();
            }
            else
            {
                throw new ArgumentException();
            }

            while (true) ;
        }
    }
}
