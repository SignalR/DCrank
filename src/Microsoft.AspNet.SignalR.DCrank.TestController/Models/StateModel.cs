using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.DCrank.Crank;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Models
{
    public class StateModel
    {
        private const string _dcrankEndpoint = "/_dcrank";

        private readonly ConcurrentDictionary<string, AgentState> _agentStates;
        private readonly IHubConnectionContext<dynamic> _clientHubConnectionContext;
        private readonly object _runLock = new object();

        private RunState _runState;
        private RunDefinition _currentRunDefinition;
        private DateTime _sendStartTime;

        static StateModel()
        {
            Instance = new StateModel();
        }

        public static StateModel Instance { get; private set; }

        private StateModel()
        {
            _agentStates = new ConcurrentDictionary<string, AgentState>();
            _runState = RunState.Idle;
            _currentRunDefinition = null;
            _clientHubConnectionContext = GlobalHost.ConnectionManager.GetHubContext<ControllerHub>().Clients;
        }

        public void AddHeartbeatInformation(string agentId, AgentHeartbeatInformation heartbeat)
        {
            var agentState = new AgentState(agentId, heartbeat);

            _agentStates.AddOrUpdate(agentId, agentState, (key, oldValue) => agentState);

            UpdateState();
        }

        public RunState GetRunState()
        {
            return _runState;
        }

        public RunDefinition GetRunDefinition()
        {
            RunDefinition runDefinition;
            lock (_runLock)
            {
                runDefinition = _currentRunDefinition;
            }
            return runDefinition;
        }

        public bool StartRun(RunDefinition runDefinition)
        {
            lock (_runLock)
            {
                if (_currentRunDefinition != null)
                {
                    return false;
                }

                _currentRunDefinition = runDefinition;
                var liveAgents = _agentStates.Values.Where(agent => agent.IsAlive()).Select(agent => agent.AgentId).ToList();
                var numberOfAgents = liveAgents.Count;

                var targetAddress = (string)runDefinition.RunParameters.First(parameter => parameter.Label.Equals("Target URL")).Value;
                var workersPerAgent = (int)runDefinition.RunParameters.First(parameter => parameter.Label.Equals("Workers per agent")).Value;
                var numberOfConnections = (int)runDefinition.RunParameters.First(parameter => parameter.Label.Equals("Connections")).Value;

                if (numberOfConnections != 0 && numberOfAgents != 0)
                {
                    var connectionsPerAgent = numberOfConnections / numberOfAgents;
                    var remainingConnections = numberOfConnections % numberOfAgents;

                    _clientHubConnectionContext.Client(liveAgents.First()).startWorkers(targetAddress, workersPerAgent, connectionsPerAgent + remainingConnections);

                    _clientHubConnectionContext.Clients(liveAgents.Skip(1).ToList()).startWorkers(targetAddress, workersPerAgent, connectionsPerAgent);
                }

                Task.Run(async () =>
                {
                    // Get connection string from the database
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(targetAddress + _dcrankEndpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var jsonData = JObject.Parse(data);
                            var connectionString = jsonData["DatabaseConnectionString"].ToObject<string>();
                            PerformanceCounters.Instance.Start(connectionString);
                        }
                        else
                        {
                            _clientHubConnectionContext.All.updatePerfCounters(new { PerformaceCountersUnavailable = "404 Not Found Error" });
                        }
                    }
                });
                return true;
            }
        }

        public bool TerminateRun()
        {
            lock (_runLock)
            {
                if (_currentRunDefinition == null)
                {
                    return false;
                }

                _currentRunDefinition = null;
                _clientHubConnectionContext.All.killConnections();
                return true;
            }
        }

        private void UpdateState()
        {
            var newState = RunState.Unknown;

            var agentStates = _agentStates.Values.Where(agent => agent.IsAlive()).Select(agent => agent.RunState).ToList();

            if (agentStates.Any(state => state == RunState.RampingUp))
            {
                newState = RunState.RampingUp;
            }
            else if (agentStates.Any(state => state == RunState.RampingDown))
            {
                newState = RunState.RampingDown;
            }
            else if (agentStates.Any(state => state == RunState.Sending))
            {
                newState = RunState.Sending;
                lock (_runLock)
                {
                    OnSend();
                }
            }
            else if (agentStates.All(state => state == RunState.Connected))
            {
                newState = RunState.Connected;
            }
            else if (agentStates.All(state => state == RunState.Idle))
            {
                newState = RunState.Idle;
            }

            lock (_runLock)
            {
                if (newState != _runState)
                {
                    OnStateChanged(_runState, newState);
                }
                _runState = newState;
            }
        }

        private void OnStateChanged(RunState oldState, RunState newState)
        {
            if (oldState == RunState.RampingUp && newState == RunState.Connected)
            {
                lock (_runLock)
                {
                    _sendStartTime = DateTime.UtcNow;
                    var messageSize = (int)_currentRunDefinition.RunParameters.First(parameter => parameter.Label.Equals("Message Size")).Value;
                    var messageRate = (double)_currentRunDefinition.RunParameters.First(parameter => parameter.Label.Equals("Send Interval")).Value;
                    _clientHubConnectionContext.All.startTest(messageSize, messageRate);
                }
            }
            else if (oldState != RunState.Idle && newState == RunState.Idle)
            {
                lock (_runLock)
                {
                    PerformanceCounters.Instance.Stop();
                    _currentRunDefinition = null;
                }
            }
        }

        private void OnSend()
        {
            var sendDuration = _currentRunDefinition.RunParameters.First(parameter => parameter.Label.Equals("Send Duration")).Value;

            if (DateTime.UtcNow.Subtract(_sendStartTime).Seconds >= sendDuration)
            {
                _clientHubConnectionContext.All.stopWorkers();
            }
        }

    }
}