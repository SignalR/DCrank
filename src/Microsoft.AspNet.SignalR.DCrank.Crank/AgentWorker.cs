﻿using System;
using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentWorker
    {
        private readonly Process _workerProcess;
        private readonly IAgent _agent;

        public AgentWorker(ProcessStartInfo startInfo, IAgent agent)
        {
            _workerProcess = new Process();
            _workerProcess.StartInfo = startInfo;
            _workerProcess.EnableRaisingEvents = true;
            _workerProcess.Exited += OnExited;
            _agent = agent;
        }

        public int Id { get; private set; }

        public StatusInformation StatusInformation { get; set; }

        public Action<int, Exception> OnError;

        public Action<int> OnExit;

        public IWorker Worker { get; private set; }

        public WorkerHeartbeatInformation GetHeartbeatInformation()
        {
            return new WorkerHeartbeatInformation
            {
                Id = Id,
                ConnectedCount = StatusInformation.ConnectedCount,
                DisconnectedCount = StatusInformation.DisconnectedCount,
                ReconnectingCount = StatusInformation.ReconnectingCount,
                TargetConnectionCount = StatusInformation.TargetConnectionCount
            };
        }

        public bool Start()
        {
            bool success = _workerProcess.Start();

            if (success)
            {
                Id = _workerProcess.Id;

                var receiver = new AgentReceiver(_workerProcess.StandardOutput, _agent);
                receiver.Start();
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
    }
}
