using System;
using System.Diagnostics;
using Newtonsoft.Json;

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

        public IWorker Worker { get; private set; }

        public bool Start()
        {
            bool success = _workerProcess.Start();

            if (success)
            {
                Id = _workerProcess.Id;
                _workerProcess.BeginOutputReadLine();
                _workerProcess.BeginErrorReadLine();

                Worker = new WorkerSender(_workerProcess.StandardInput);
            }

            return success;
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
