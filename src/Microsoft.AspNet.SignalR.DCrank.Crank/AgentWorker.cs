using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentWorker
    {
        private Process _workerProcess;
        private ProcessStartInfo _workerProcessStartInfo;

        public AgentWorker(ProcessStartInfo startInfo)
        {
            _workerProcessStartInfo = startInfo;

            _workerProcess = new Process();
            _workerProcess.StartInfo = _workerProcessStartInfo;
            _workerProcess.EnableRaisingEvents = true;
            _workerProcess.OutputDataReceived += OnOutputDataReceived;
            _workerProcess.Exited += OnExited;
        }

        public int Id { get; private set; }

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
