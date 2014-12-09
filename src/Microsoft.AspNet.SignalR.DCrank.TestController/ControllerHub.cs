using Microsoft.AspNet.SignalR.DCrank.Crank;
using Microsoft.AspNet.SignalR.DCrank.TestController.Models;
namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        public void PingAgent(string agentId, int value)
        {
            Clients.Client(agentId).pingAgent(value);
        }

        public void PingWorker(string agentId, int workerId, int value)
        {
            Clients.Client(agentId).pingWorker(workerId, value);
        }

        public void PongAgent(int value)
        {
            Clients.All.agentPongResponse(Context.ConnectionId, value);
        }

        public void PongWorker(int workerId, int value)
        {
            Clients.All.workerPongResponse(Context.ConnectionId, workerId, value);
        }

        public void LogAgent(string message)
        {
            Clients.All.agentsLog(Context.ConnectionId, message);
        }

        public void LogWorker(int workerId, string message)
        {
            Clients.All.workersLog(Context.ConnectionId, workerId, message);
        }

        public void AgentHeartbeat(AgentHeartbeatInformation heartbeatInformation)
        {
            StateModel.Instance.AddHeartbeatInformation(Context.ConnectionId, heartbeatInformation);
            Clients.All.agentConnected(Context.ConnectionId, heartbeatInformation);
        }
    }
}