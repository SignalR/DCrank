using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentWorker
    {
        private Process _workerProcess;
        private ProcessStartInfo _workerProcessStartInfo;
        private readonly JsonSerializer _serializer;

        public AgentWorker(ProcessStartInfo startInfo)
        {
            _workerProcessStartInfo = startInfo;

            _workerProcess = new Process();
            _workerProcess.StartInfo = _workerProcessStartInfo;
            _workerProcess.EnableRaisingEvents = true;
            _workerProcess.Exited += OnExited;

            _serializer = new JsonSerializer();
        }


        public int Id { get; private set; }

        public Action<int, Message> OnMessage;

        public Action<int, Exception> OnError;

        public Action<int> OnExit;

        public Action<int, string> OnLog;


        public bool Start()
        {
            bool success = _workerProcess.Start();

            if (success)
            {
                Id = _workerProcess.Id;

                ProcessingMessages();
            }

            return success;
        }

        public void Ping(int value)
        {
            var message = new Message()
            {
                Command = "ping",
                Value = value
            };

            _serializer.Serialize(new JsonTextWriter(_workerProcess.StandardInput), message);
        }

        public void Kill()
        {
            _workerProcess.Kill();
        }

        private void OnExited(object sender, EventArgs args)
        {
            OnExit(Id);
        }

        private void ProcessingMessages()
        {
            Task.Run(() =>
            {
                while (!_workerProcess.HasExited)
                {
                    try
                    {
                        var message = _serializer.Deserialize<Message>(new JsonTextReader(_workerProcess.StandardOutput));
                        OnMessage(Id, message);
                    }
                    catch (Exception ex)
                    {
                        OnError(Id, ex);
                        break;
                    }
                }
            });

            Task.Run(async () =>
            {
                while (!_workerProcess.HasExited)
                {
                    string output = await _workerProcess.StandardError.ReadLineAsync();
                    if (!string.IsNullOrEmpty(output))
                    {
                        OnLog(Id, output);
                    }
                }
            });
        }

        private static void Log(string format, params object[] arguments)
        {
            Trace.WriteLine(string.Format(format, arguments));
        }
    }
}
