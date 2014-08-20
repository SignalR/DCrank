using CmdLine;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    [CommandLineArguments(Program = "DCrank")]
    public class DCrankArguments
    {
        [CommandLineParameter(Command = "?", Name = "Help", Default = false, Description = "Show Help", IsHelp = true)]
        public bool Help { get; set; }

        [CommandLineParameter(Command = "Mode", Required = true, Description = "DCrank operating mode (Agent or Worker).")]
        public string Mode { get; set; }

        [CommandLineParameter(Command = "ControllerUrl", Required = false, Default = "http://localhost:17063", Description = "URL for Test Controller (Agent mode only).")]
        public string ControllerUrl { get; set; }

        [CommandLineParameter(Command = "ParentPid", Required = false, Description = "Process ID of calling agent (Worker mode only).")]
        public int ParentPid { get; set; }
    }
}
