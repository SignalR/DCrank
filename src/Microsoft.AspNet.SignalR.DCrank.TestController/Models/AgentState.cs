using System;
using System.Linq;
using Microsoft.AspNet.SignalR.DCrank.Crank;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Models
{
    public class AgentState
    {
        private readonly DateTime _time;

        public AgentState(string agentId, AgentHeartbeatInformation heartbeatInformation)
        {
            AgentId = agentId;
            _time = DateTime.UtcNow;

            var connectionsConnected = heartbeatInformation.Workers.Sum(worker => worker.ConnectedCount);

            if (heartbeatInformation.ApplyingLoad)
            {
                RunState = RunState.Sending;
            }
            else if (heartbeatInformation.TotalConnectionsRequested != 0 &&
                     connectionsConnected == heartbeatInformation.TotalConnectionsRequested)
            {
                RunState = RunState.Connected;
            }
            else if (connectionsConnected < heartbeatInformation.TotalConnectionsRequested)
            {
                RunState = RunState.RampingUp;
            }
            else if (connectionsConnected == 0 && heartbeatInformation.TotalConnectionsRequested == 0)
            {
                RunState = RunState.Idle;
            }
            else
            {
                RunState = RunState.RampingDown;
            }
        }

        public string AgentId { get; private set; }

        public RunState RunState { get; private set; }

        public bool IsAlive()
        {
            return DateTime.UtcNow.Subtract(_time).Seconds < 15;
        }
    }
}