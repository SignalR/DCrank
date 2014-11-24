
namespace Microsoft.AspNet.SignalR.DCrank.TestController.Models
{
    public enum RunState
    {
        Idle,
        RampingUp,
        Connected,
        Sending,
        RampingDown,
        Unknown
    }
}