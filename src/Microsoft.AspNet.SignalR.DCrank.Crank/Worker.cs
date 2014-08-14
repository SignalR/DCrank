using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Worker
    {
        private readonly Process _agentProcess;
        private List<Client> _clients;
        private int targetConnectionCount = 0;

        public Worker(int agentProcessId)
        {
            _agentProcess = Process.GetProcessById(agentProcessId);
            _clients = new List<Client>();
        }

        public async Task Run()
        {
            _agentProcess.EnableRaisingEvents = true;
            _agentProcess.Exited += OnExited;

            Log("Worker created");

            Task.Run(() => SendStatusUpdate());

            bool workerStopped = false;

            while (!workerStopped)
            {
                var messageString = await Console.In.ReadLineAsync();
                var message = JsonConvert.DeserializeObject<Message>(messageString);

                try
                {
                    switch (message.Command.ToLowerInvariant())
                    {
                        case "ping":
                            var value = message.Value.ToObject<int>();
                            Log("Worker received {0} command with value {1}.", message.Command, message.Value);

                            await Send("pong", value);
                            Log("Worker sent pong command with value {0}.", value);

                            break;

                        case "connect":
                            var connectArguments = new CrankArguments()
                            {
                                Url = message.Value["TargetAddress"].ToObject<string>(),
                            };

                            var numberOfConnections = message.Value["NumberOfConnections"].ToObject<int>();
                            targetConnectionCount += numberOfConnections;
                            for (int count = 0; count < numberOfConnections; count++)
                            {
                                var client = new Client();

                                client.OnMessage += OnMessage;
                                client.OnClosed += OnClosed;

                                await client.CreateConnection(connectArguments);
                                _clients.Add(client);
                            }

                            Log("Connections connected succesfully");

                            break;

                        case "starttest":
                            var crankArguments = message.Value.ToObject<CrankArguments>();

                            Log("Worker received {0} command with value.", message.Command);

                            foreach (var client in _clients)
                            {
                                client.StartTest(crankArguments);
                            }

                            Log("Test started succesfully");
                            break;

                        case "stop":
                            targetConnectionCount = 0;
                            foreach (var client in _clients)
                            {
                                client.StopConnection();
                            }

                            _clients.Clear();
                            Log("Connections stopped succesfully");

                            workerStopped = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Send("Log", string.Format("Worker encountered the following exception: {0}", ex.Message));
                }
            }
        }

        private void OnMessage(string message)
        {
            Send("Log", string.Format("Worker received following message from server: {0}", message));
        }

        private void OnClosed()
        {
            // Indicates that the connection was closed
        }

        private void OnExited(object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        private async void Log(string format, params object[] arguments)
        {
            await Send("Log", string.Format(format, arguments));
        }

        private async Task Send(string command, object value)
        {
            var message = new Message
            {
                Command = command,
                Value = JToken.FromObject(value)
            };

            var messageString = JsonConvert.SerializeObject(message);
            await Console.Out.WriteLineAsync(messageString);
        }

        private void SendStatusUpdate()
        {
            while (true)
            {
                int connectedCount = 0, disconnectedCount = 0, reconnectingCount = 0;

                foreach (var client in _clients)
                {
                    switch (client.ConnectionState)
                    {
                        case ConnectionState.Connected:
                            connectedCount++;
                            break;
                        case ConnectionState.Disconnected:
                            disconnectedCount++;
                            break;
                        case ConnectionState.Reconnecting:
                            reconnectingCount++;
                            break;
                    }
                }

                Send("status", new
                {
                    ConnectedCount = connectedCount,
                    DisconnectedCount = disconnectedCount,
                    ReconnectingCount = reconnectingCount,
                    TargetConnectionCount = targetConnectionCount
                });

                // Sending once per 5 seconds to avoid overloading the Test Controller
                Thread.Sleep(5000);
            }

        }
    }
}
