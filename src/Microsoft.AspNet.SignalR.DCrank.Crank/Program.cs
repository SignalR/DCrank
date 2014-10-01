using System;
using CmdLine;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var arguments = CommandLine.Parse<DCrankArguments>();
                switch (arguments.Mode.ToLowerInvariant())
                {
                    case "agent":
                        var agent = new Agent();
                        var runner = new HubRunner(agent, arguments.ControllerUrl);
                        runner.Run().Wait();
                        break;
                    case "worker":
                        var worker = new Worker(arguments.ParentPid);
                        worker.Run().Wait();
                        break;
                    default:
                        throw new ArgumentException(string.Format("Invalid value for Mode \"{0}\"", arguments.Mode));
                }
            }
            catch (CommandLineException ex)
            {
                Console.WriteLine(ex.ArgumentHelp.Message);
                Console.WriteLine(ex.ArgumentHelp.GetHelpText(Console.BufferWidth));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
