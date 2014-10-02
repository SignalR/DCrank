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
                    case "commandline":
                        StartCommandLine(arguments);
                        break;
                    case "agent":
                        StartAgent(arguments);
                        break;
                    case "worker":
                        StartWorker(arguments);
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

        private static void StartCommandLine(DCrankArguments arguments)
        {
            var agent = new Agent();
            var runner = new Runner(agent, arguments);
            runner.Run().Wait();
        }

        private static void StartAgent(DCrankArguments arguments)
        {
            var agent = new Agent();
            var runner = new HubRunner(agent, arguments.ControllerUrl);
            runner.Run().Wait();
        }

        private static void StartWorker(DCrankArguments arguments)
        {
            var worker = new Worker(arguments.ParentPid);
            worker.Run().Wait();
        }
    }
}
