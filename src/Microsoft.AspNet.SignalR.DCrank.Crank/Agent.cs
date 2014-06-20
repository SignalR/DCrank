using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Agent
    {
        private const string _url = "http://localhost:17063";
        private const string _fileName = "Microsoft.AspNet.SignalR.DCrank.Crank.exe";
        private const string _hubName = "ControllerHub";
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;
        private readonly ProcessStartInfo _startInfo;
        private readonly Dictionary<int, Process> _processes;
        private readonly JsonSerializer _serializer;

        public Agent()
        {
            _connection = new HubConnection(_url);
            _proxy = _connection.CreateHubProxy(_hubName);
            _startInfo = new ProcessStartInfo()
            {
                FileName = _fileName,
                Arguments = "",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            _processes = new Dictionary<int, Process>();
            _serializer = new JsonSerializer();
        }

        public void Initialize()
        {
            _proxy.On<int>("pingAgent", value =>
            {
                _proxy.Invoke("pongAgent", value);
            });

            _proxy.On("startWorker", () =>
            {
                var worker = StartWorker();
                StartReadLoop(worker);
                _proxy.Invoke("workerHeartbeat", worker.Id);
            });

            _proxy.On<int, int>("pingWorker", (processId, value) =>
            {
                Process process;
                if (_processes.TryGetValue(processId, out process))
                {
                    _serializer.Serialize(new JsonTextWriter(process.StandardInput), new Message() { Command = "ping", Value = value });
                }
            });
        }

        public async Task Run()
        {
            await _connection.Start();
            await _proxy.Invoke("agentHeartbeat");
        }

        private Process StartWorker()
        {
            var process = Process.Start(_startInfo);
            _processes.Add(process.Id, process);
            return process;
        }

        private void StartReadLoop(Process process)
        {
            var id = process.Id;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var message = _serializer.Deserialize<Message>(new JsonTextReader(process.StandardOutput));
                    if (string.Equals(message.Command, "pong"))
                    {
                        _proxy.Invoke("pongWorker", id, message.Value);
                    }
                }
            });
        }
    }
}
