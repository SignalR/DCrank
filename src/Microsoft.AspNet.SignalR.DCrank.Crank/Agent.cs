using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Agent
    {
        private const string _url = "http://localhost:17063";
        private const string _fileName = "Microsoft.AspNet.SignalR.DCrank.Crank.exe";
        private const string _hubName = "ControllerHub";
        private readonly ProcessStartInfo _startInfo;
        private readonly Dictionary<int, Process> _processes;
        private readonly JsonSerializer _serializer;
        private HubConnection _connection;
        private IHubProxy _proxy;

        public Agent()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            _startInfo = new ProcessStartInfo()
            {
                FileName = _fileName,
                Arguments = "",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _processes = new Dictionary<int, Process>();
            _serializer = new JsonSerializer();

            Trace.WriteLine("Agent created");
        }

        private void InitializeProxy()
        {
            _proxy.On<int>("pingAgent", value =>
            {
                LogAgent("Agent received pingAgent with value {0}.", value);
                InvokeController("pongAgent", value);
            });

            _proxy.On("startWorker", () =>
            {
                LogAgent("Agent received startWorker command.");

                var worker = StartWorker();
                LogAgent("Agent started worker {0}.", worker.Id);

                StartReadLoop(worker);
                LogAgent("Agent started listening to worker {0}.", worker.Id);

                InvokeController("workerHeartbeat", worker.Id);
            });

            _proxy.On<int, int>("pingWorker", (processId, value) =>
            {
                LogAgent("Agent received pingWorker for Worker {0} with value {1}.", processId, value);
                Process process;
                if (_processes.TryGetValue(processId, out process))
                {
                    _serializer.Serialize(new JsonTextWriter(process.StandardInput), new Message() { Command = "ping", Value = value });
                    LogAgent("Agent sent ping command to Worker {0} with value {1}.", processId, value);
                }
                else
                {
                    LogAgent("Agent failed to send ping command, Worker {0} not found.", processId);
                }
            });
        }

        public async Task Run()
        {
            while (true)
            {
                try
                {
                    using (_connection = new HubConnection(_url))
                    {
                        _proxy = _connection.CreateHubProxy(_hubName);
                        InitializeProxy();

                        Trace.WriteLine("Attempting to connect to TestController");
                        try
                        {
                            await _connection.Start();
                            LogAgent("Agent connected to TestController.", _connection.ConnectionId);

                            while (_connection.State == ConnectionState.Connected)
                            {
                                await InvokeController("agentHeartbeat");
                                await Task.Delay(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(string.Format("Agent failed to connect to server: {0}", ex.Message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Connection lost: {0}", ex.Message));
                }
                await Task.Delay(10000);
            }
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
                        LogAgent("Agent received pong message from Worker {0} with value {1}.", process.Id, message.Value);
                        InvokeController("pongWorker", id, message.Value).Wait();
                    }
                }
            });
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    LogWorker(process.Id, await process.StandardError.ReadLineAsync());
                }
            });
        }

        private void LogWorker(int workerId, string format, params object[] arguments)
        {
            var prefix = string.Format("({0}, {1}) ", _connection.ConnectionId, workerId);
            var message = "[" + DateTime.Now.ToString() + "] " + string.Format(format, arguments);
            Trace.WriteLine(prefix + message);

            try
            {
                _proxy.Invoke("LogWorker", workerId, message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(prefix + string.Format("LogWorker threw an exception: {0}", ex.Message));
            }
        }

        private void LogAgent(string format, params object[] arguments)
        {
            var prefix = string.Format("({0}) ", _connection.ConnectionId, DateTime.Now);
            var message = "[" + DateTime.Now.ToString() + "] " + string.Format(format, arguments);
            Trace.WriteLine(prefix + message);

            try
            {
                _proxy.Invoke("LogAgent", message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(prefix + string.Format("LogAgent threw an exception: {0}", ex.Message));
            }
        }

        private async Task InvokeController(string command, params object[] arguments)
        {
            var commandString = command + "(" + string.Join(", ", arguments) + ")";

            try
            {
                await _proxy.Invoke(command, arguments);
                LogAgent("Agent completed call to TestController: {0}", commandString);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Agent attempted call to TestController: {0}. Exception: {1}", command, ex.Message));
            }
        }
    }
}
