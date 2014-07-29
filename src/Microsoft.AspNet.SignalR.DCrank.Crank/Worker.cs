using System;
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
        private Client _client;

        public Worker(int agentProcessId)
        {
            _agentProcess = Process.GetProcessById(agentProcessId);
            _client = new Client();
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
                    Send("status", new
                    {
                        ConnectedCount = (_client.ConnectionState == ConnectionState.Connected)? 1 : 0,
                        DisconnectedCount = (_client.ConnectionState == ConnectionState.Disconnected) ? 1 : 0,
                        ReconnectingCount = (_client.ConnectionState == ConnectionState.Reconnecting) ? 1 : 0
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

                            _client.OnMessage += OnMessage;
                            _client.OnClosed += OnClosed;

                            await _client.CreateConnection(crankArguments);
                            Log("Connection started succesfully");

                            _client.StartTest(crankArguments);

                            break;

                        case "stop":
                            _client.StopConnection();
                            Log("Connection stopped succesfully");

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
