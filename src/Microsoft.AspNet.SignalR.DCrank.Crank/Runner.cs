using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Runner : IRunner
    {
        private readonly Agent _agent;
        private readonly string _targetUrl;
        private readonly int _numberOfWorkers;
        private readonly int _numberOfConnections;
        private readonly int _sendDurationSeconds;

        public Runner(Agent agent, DCrankArguments arguments)
        {
            _agent = agent;
            _targetUrl = arguments.TargetUrl;
            _numberOfWorkers = arguments.Workers;
            _numberOfConnections = arguments.Connections;
            _sendDurationSeconds = arguments.SendDuration;
        }

        public async Task Run()
        {
            _agent.Runner = this;

            _agent.StartWorkers(_targetUrl, _numberOfWorkers, _numberOfConnections);

            // Begin writing worker status information
            var writeStatusCts = new CancellationTokenSource();
            var writeStatusTask = WriteConnectionStatus(writeStatusCts.Token);

            // Wait until all connections are connected
            while (_agent.GetWorkerStatus().Aggregate(0, (state, status) => state + status.Value.ConnectedCount) <
                _agent.TotalConnectionsRequested)
            {
                await Task.Delay(1000);
            }

            // Stay connected for the duration of the send phase
            await Task.Delay(TimeSpan.FromSeconds(_sendDurationSeconds));

            // Disconnect
            await _agent.StopWorkers();

            // Stop writing worker status information
            writeStatusCts.Cancel();
            await writeStatusTask;
        }

        private async Task WriteConnectionStatus(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var statusDictionary = _agent.GetWorkerStatus();
                    foreach (var key in statusDictionary.Keys)
                    {
                        Trace.WriteLine(string.Format("({0}) {1}", key, JsonConvert.SerializeObject(statusDictionary[key])));
                    }
                    await Task.Delay(1000);
                }
            });
        }

        public Task PongWorker(int workerId, int value)
        {
            throw new NotImplementedException();
        }

        public async Task LogAgent(string format, params object[] arguments)
        {
            Trace.WriteLine(string.Format(format, arguments));
        }

        public async Task LogWorker(int workerId, string format, params object[] arguments)
        {
            Trace.WriteLine(string.Format("({0}) {1}", workerId, string.Format(format, arguments)));
        }
    }
}
