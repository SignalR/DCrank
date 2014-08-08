
using System.Threading;
using System.Threading.Tasks;
namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        private readonly string _uiGroup = "Dashboard";
        private readonly int numberOfWorkersPerAgent = 3; // Default value - can be changed/made configurable as needed

        public override async Task OnConnected()
        {
            var isDashboard = Context.Request.QueryString["UI"] == "1";
            if (isDashboard)
            {
                await Groups.Add(Context.ConnectionId, _uiGroup);
            }
        }

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

        public void AgentHeartbeat(object heartbeatInformation)
        {
            Clients.All.agentConnected(Context.ConnectionId, heartbeatInformation);
        }

        public void StartWorker(string connectionId, int numberOfWorkersToStart, int numberOfConnectionsPerWorker)
        {
            Clients.Client(connectionId).startWorkers(numberOfWorkersToStart, numberOfConnectionsPerWorker);
        }

        public void KillWorker(string agentId, int workerId)
        {
            Clients.Client(agentId).killWorker(workerId);
        }

        public void StopWorker(string agentId, int workerId)
        {
            Clients.Client(agentId).stopWorker(workerId);
        }

        public void KillWorkers(string agentId, int numberOfWorkersToKill)
        {
            Clients.Client(agentId).killWorkers(numberOfWorkersToKill);
        }

        public void SendTestInfoManual(string targetAddress, int messageSize, int messageRate)
        {
            Clients.All.startTest(targetAddress, messageSize, messageRate);
        }

        public void SetUpTest(string targetAddresss, int numberOfConnections, int numberOfAgents)
        {
            if (numberOfConnections != 0 && numberOfAgents != 0)
            {
                int numberOfConnectionsPerWorker = numberOfConnections / (numberOfWorkersPerAgent * numberOfAgents);
                Clients.All.startWorkers(targetAddresss, numberOfWorkersPerAgent, numberOfConnectionsPerWorker);
            }
        }

        public void StartTest(int messageSize, int messageRate)
        {
            Clients.All.startTest(messageSize, messageRate);
        }

        public void KillConnections()
        {
            Clients.All.killWorkers(100);
        }

        public void LogAgent(string message)
        {
            Clients.All.agentsLog(Context.ConnectionId, message);
        }

        public void LogWorker(int workerId, string message)
        {
            Clients.All.workersLog(Context.ConnectionId, workerId, message);
        }
    }
}