using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Agent : IAgent
    {
        private const string _fileName = "crank.exe";
        private readonly string _hostName;

        private readonly ConcurrentDictionary<int, AgentWorker> _workers;

        public Agent()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            _hostName = Dns.GetHostName();

            _workers = new ConcurrentDictionary<int, AgentWorker>();

            Trace.WriteLine("Agent created");
        }

        public IRunner Runner { get; set; }

        public string TargetAddress { get; private set; }

        public int TotalConnectionsRequested { get; private set; }

        public bool ApplyingLoad { get; private set; }

        public AgentHeartbeatInformation GetHeartbeatInformation()
        {
            return new AgentHeartbeatInformation
            {
                HostName = _hostName,
                TargetAddress = TargetAddress,
                TotalConnectionsRequested = TotalConnectionsRequested,
                ApplyingLoad = ApplyingLoad,
                Workers = _workers.Select(worker => worker.Value.GetHeartbeatInformation()).ToList()
            };
        }

        public Dictionary<int, StatusInformation> GetWorkerStatus()
        {
            return _workers.Values.ToDictionary(
                k => k.Id,
                v => v.StatusInformation);
        }

        private AgentWorker CreateWorker()
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = _fileName,
                Arguments = string.Format("/Mode:worker /ParentPid:{0}", Process.GetCurrentProcess().Id),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var worker = new AgentWorker(startInfo, this);

            worker.StatusInformation = new StatusInformation();

            worker.Start();

            worker.OnError += OnError;
            worker.OnExit += OnExit;

            _workers.TryAdd(worker.Id, worker);

            return worker;
        }

        private async Task StartWorker(int id, string targetAddress, int numberOfConnectionsPerWorker)
        {
            AgentWorker worker;
            if (_workers.TryGetValue(id, out worker))
            {
                await worker.Worker.Connect(targetAddress, numberOfConnectionsPerWorker);
            }
        }

        public void StartWorkers(string targetAddress, int numberOfWorkers, int numberOfConnections)
        {
            TargetAddress = targetAddress;
            TotalConnectionsRequested = numberOfConnections;

            var connectionsPerWorker = numberOfConnections / numberOfWorkers;
            var remainingConnections = numberOfConnections % numberOfWorkers;

            Task.Run(() =>
            {
                Parallel.For(0, numberOfWorkers, async index =>
                {
                    var worker = CreateWorker();

                    if (index == 0)
                    {
                        await StartWorker(worker.Id, targetAddress, connectionsPerWorker + remainingConnections);
                    }
                    else
                    {
                        await StartWorker(worker.Id, targetAddress, connectionsPerWorker);
                    }

                    await Runner.LogAgent("Agent started listening to worker {0} ({1} of {2}).", worker.Id, index, numberOfWorkers);
                });
            });
        }

        public void KillWorker(int workerId)
        {
            AgentWorker worker;

            if (_workers.TryGetValue(workerId, out worker))
            {
                worker.Kill();
                Runner.LogAgent("Agent killed Worker {0}.", workerId);
            }
        }

        public void KillWorkers(int numberOfWorkersToKill)
        {
            var keys = _workers.Keys.Take(numberOfWorkersToKill).ToList();

            foreach (var key in keys)
            {
                AgentWorker worker;
                if (_workers.TryGetValue(key, out worker))
                {
                    worker.Kill();
                    Runner.LogAgent("Agent killed Worker {0}.", key);
                }
            }
        }

        public void KillConnections()
        {
            var keys = _workers.Keys.ToList();

            foreach (var key in keys)
            {
                AgentWorker worker;
                if (_workers.TryGetValue(key, out worker))
                {
                    worker.Kill();
                    Runner.LogAgent("Agent killed Worker {0}.", key);
                }
            }

            TotalConnectionsRequested = 0;
            ApplyingLoad = false;
        }

        public void PingWorker(int workerId, int value)
        {
            AgentWorker worker;

            if (_workers.TryGetValue(workerId, out worker))
            {
                worker.Worker.Ping(value);
                Runner.LogAgent("Agent sent ping command to Worker {0} with value {1}.", workerId, value);
            }
            else
            {
                Runner.LogAgent("Agent failed to send ping command, Worker {0} not found.", workerId);
            }
        }

        public void StartTest(int messageSize, int sendIntervalMiliseconds)
        {
            ApplyingLoad = true;

            Task.Run(() =>
            {
                foreach (var worker in _workers.Values)
                {
                    worker.Worker.StartTest(sendIntervalMiliseconds, messageSize);
                }
            });
        }

        public void StopWorker(int workerId)
        {
            AgentWorker worker;
            if (_workers.TryGetValue(workerId, out worker))
            {
                worker.Worker.Stop();
            }
        }

        public async Task StopWorkers()
        {
            var keys = _workers.Keys.ToList();

            foreach (var key in keys)
            {
                AgentWorker worker;
                if (_workers.TryGetValue(key, out worker))
                {
                    await worker.Worker.Stop();
                    await Runner.LogAgent("Agent stopped Worker {0}.", key);
                }
            }
            TotalConnectionsRequested = 0;
            ApplyingLoad = false;

            // Wait for workers to terminate
            while (_workers.Count > 0)
            {
                await Task.Delay(1000);
            }
        }

        public async Task Pong(int id, int value)
        {
            await Runner.LogAgent("Agent received pong message from Worker {0} with value {1}.", id, value);
            await Runner.PongWorker(id, value);
        }

        public async Task Log(int id, string text)
        {
            await Runner.LogWorker(id, text);
        }

        public async Task Status(
            int id,
            StatusInformation statusInformation)
        {
            await Runner.LogAgent("Agent received status message from Worker {0}.", id);

            AgentWorker worker;
            if (_workers.TryGetValue(id, out worker))
            {
                worker.StatusInformation = statusInformation;
            }
        }

        private void OnError(int workerId, Exception ex)
        {
            Runner.LogWorker(workerId, ex.Message);
        }

        private void OnExit(int workerId)
        {
            AgentWorker worker;
            _workers.TryRemove(workerId, out worker);
        }
    }
}
