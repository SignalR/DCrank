
namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class StatusInformation
    {
        public int ConnectedCount { get; set; }
        public int DisconnectedCount { get; set; }
        public int ReconnectingCount { get; set; }
        public int TargetConnectionCount { get; set; }
    }
}
