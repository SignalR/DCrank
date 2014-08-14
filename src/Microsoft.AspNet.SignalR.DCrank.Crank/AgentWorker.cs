using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentWorker
    {
        private readonly Process _workerProcess;

        public AgentWorker(ProcessStartInfo startInfo)
        {
            _workerProcess = new Process();
            _workerProcess.StartInfo = startInfo;
            _workerProcess.EnableRaisingEvents = true;
            _workerProcess.OutputDataReceived += OnOutputDataReceived;
            _workerProcess.Exited += OnExited;
        }

        public int Id { get; private set; }

        public int ConnectedCount { get; set; }

        public int DisconnectedCount { get; set; }

        public int ReconnectedCount { get; set; }

        public int TargetConnectionCount { get; set; }

        public Action<int, Message> OnMessage;

        public Action<int, Exception> OnError;

        public Action<int> OnExit;

        public bool Start()
        {
            bool success = _workerProcess.Start();

            if (success)
            {
                Id = _workerProcess.Id;
                _workerProcess.BeginOutputReadLine();
                _workerProcess.BeginErrorReadLine();
            }

            return success;
        }

        public async Task Send(string command, object value)
        {
            var message = new Message
            {
                Command = command,
                Value = JToken.FromObject(value)
            };

            var messageString = JsonConvert.SerializeObject(message);

            await _workerProcess.StandardInput.WriteLineAsync(messageString);
        }

        public void Kill()
        {
            _workerProcess.Kill();
        }

        public async Task Stop()
        {
            await Send("stop", new object());
        }

        public async Task Connect(string targetAddress, int numberOfConnections)
        {
            await Send("connect", new
            {
                TargetAddress = targetAddress,
                NumberOfConnections = numberOfConnections
            });
        }

        public async Task StartTest(int sendBytes, int messagesPerSecond)
        {
            var parameters = new CrankArguments();
            parameters.SendInterval = (1000 / messagesPerSecond);
            parameters.SendBytes = sendBytes;

            await Send("starttest", parameters);
        }

        private void OnExited(object sender, EventArgs args)
        {
            OnExit(Id);
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var messageString = e.Data;

            if (string.IsNullOrEmpty(messageString))
            {
                return;
            }

            var message = JsonConvert.DeserializeObject<Message>(messageString);

            OnMessage(Id, message);
        }
    }
}
