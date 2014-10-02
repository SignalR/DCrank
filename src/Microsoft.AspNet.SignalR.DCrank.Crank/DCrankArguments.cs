using CmdLine;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    [CommandLineArguments(Program = "DCrank")]
    public class DCrankArguments
    {
        [CommandLineParameter(Command = "?", Name = "Help", Default = false, Description = "Show Help", IsHelp = true)]
        public bool Help { get; set; }

        [CommandLineParameter(Command = "Mode", Required = false, Default = "commandline",Description = "DCrank operating mode (CommandLine, Agent, or Worker).")]
        public string Mode { get; set; }

        [CommandLineParameter(Command = "ControllerUrl", Required = false, Default = "http://localhost:17063", Description = "URL for Test Controller (Agent mode only).")]
        public string ControllerUrl { get; set; }

        [CommandLineParameter(Command = "ParentPid", Required = false, Description = "Process ID of calling agent (Worker mode only).")]
        public int ParentPid { get; set; }

        [CommandLineParameter(Command = "TargetUrl", Required = false, Default = "http://localhost:24037/", Description = "The URL for the test target (CommandLine mode only).")]
        public string TargetUrl { get; set; }

        [CommandLineParameter(Command = "Workers", Required = false, Default = 1, Description = "Number of worker processes to create (CommandLine mode only).")]
        public int Workers { get; set; }

        [CommandLineParameter(Command="Connections", Required = false, Default = 100000, Description = "The number of connections to establish with the test target (CommandLine mode only).")]
        public int Connections { get; set; }

        [CommandLineParameter(Command = "SendDuration", Required = false, Default = 300, Description = "(Send phase) Duration in seconds. Default: 300 seconds")]
        public int SendDuration { get; set; }
    }
}
