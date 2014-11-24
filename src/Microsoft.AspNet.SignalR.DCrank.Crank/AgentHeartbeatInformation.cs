using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentHeartbeatInformation
    {
        public string HostName { get; set; }

        public string TargetAddress { get; set; }

        public int TotalConnectionsRequested { get; set; }

        public bool ApplyingLoad { get; set; }

        public List<WorkerHeartbeatInformation> Workers { get; set; }
    }
}
