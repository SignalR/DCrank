
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        private readonly string _uiGroup = "Dashboard";
        private readonly int _numberOfWorkersPerAgent = 3; // Default value - can be changed/made configurable as needed
        private string _connectionString;
        private SqlConnection _performanceDatabaseConnection;

        private readonly PerformanceCounters _performanceCounters;

        public ControllerHub() : this(PerformanceCounters.Instance) { }

        public ControllerHub(PerformanceCounters PerformanceCounters)
        {
            _performanceCounters = PerformanceCounters;
        }

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

        public void LogAgent(string message)
        {
            Clients.All.agentsLog(Context.ConnectionId, message);
        }

        public void LogWorker(int workerId, string message)
        {
            Clients.All.workersLog(Context.ConnectionId, workerId, message);
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

        public void StopWorkers()
        {
            Clients.All.stopWorkers();
        }

        public void KillWorkers(string agentId, int numberOfWorkersToKill)
        {
            Clients.Client(agentId).killWorkers(numberOfWorkersToKill);
        }

        public void SendTestInfoManual(string targetAddress, int messageSize, int messageRate)
        {
            Clients.All.startTest(targetAddress, messageSize, messageRate);
        }

        public async Task SetUpTest(string targetAddresss, int numberOfConnections, string[] agentIdList)
        {
            int numberOfAgents = agentIdList.Length;
            if (numberOfConnections != 0 && numberOfAgents != 0)
            {
                int numberOfConnectionsPerWorker = numberOfConnections / (_numberOfWorkersPerAgent * numberOfAgents);
                Clients.All.startWorkers(targetAddresss, _numberOfWorkersPerAgent, numberOfConnectionsPerWorker);
            }

            // Get connection string from the database
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(targetAddresss + "/dcrank");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var jsonData = JObject.Parse(data);
                    _connectionString = jsonData["ConnectionString"].ToObject<string>();
                    _performanceCounters.Start(_connectionString);
                }
            }
        }

        public void StartTest(int messageSize, int messageRate)
        {
            Clients.All.startTest(messageSize, messageRate);
        }

        public void KillConnections()
        {
            Clients.All.killConnections();
        }
    }
}