using System;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && string.Equals(args[0], "agent", StringComparison.OrdinalIgnoreCase))
            {
                var agent = new Agent();
                agent.Run();
            }
            else
            {
                Worker.Run();
            }
            while (true) ;
        }
    }
}
