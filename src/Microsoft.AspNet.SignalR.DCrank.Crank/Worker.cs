using System;
using System.Collections.Concurrent;
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
        private ICollection<Client> _clients;

        public Worker(int agentProcessId, int numberOfConnections)
        {
            _agentProcess = Process.GetProcessById(agentProcessId);
            _clients = new List<Client>();

            for (int count = 0; count < numberOfConnections; count++)
            {
                _clients.Add(new Client());
            }
        }

        public async Task Run()
        {
            _agentProcess.EnableRaisingEvents = true;
            _agentProcess.Exited += OnExited;

            Log("Worker created");

            var updateThread = new Thread(() =>
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
                        ReconnectingCount = reconnectingCount
                    });

                    Thread.Sleep(1000);
                }
            });

            updateThread.Start();

            while (true)
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

                        case "starttest":
                            var crankArguments = message.Value.ToObject<CrankArguments>();

                            Log("Worker received {0} command with value.", message.Command);

                            foreach (var client in _clients)
                            {
                                client.OnMessage += OnMessage;
                                client.OnClosed += OnClosed;

                                await client.CreateConnection(crankArguments);
                                client.StartTest(crankArguments);
                            }

                            Log("Connections started succesfully");
                            break;

                        case "stop":
                            foreach (var client in _clients)
                            {
                                client.StopConnection();
                            }

                            _clients.Clear();
                            Log("Connections stopped succesfully");
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
    }
}
