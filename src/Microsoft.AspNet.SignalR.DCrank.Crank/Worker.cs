using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Worker
    {
        private readonly Process _agentProcess;

        public Worker(int agentProcessId)
        {
            _agentProcess = Process.GetProcessById(agentProcessId);
        }

        public async Task Run()
        {
            _agentProcess.EnableRaisingEvents = true;
            _agentProcess.Exited += OnExited;

            Log("Worker created");

            while (true)
            {
                var messageString = await Console.In.ReadLineAsync();

                var message = JsonConvert.DeserializeObject<Message>(messageString);

                Log("Worker received {0} command with value {1}.", message.Command, message.Value);

                if (string.Equals(message.Command, "ping"))
                {
                    var value = message.Value.ToObject<int>();
                    await Send("pong", value);

                    Log("Worker sent pong command with value {0}.", value);
                }
            }
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
            string messageString = JsonConvert.SerializeObject(message);
            await Console.Out.WriteLineAsync(messageString);
        }
    }
}
