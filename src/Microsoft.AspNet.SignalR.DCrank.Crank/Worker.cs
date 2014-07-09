using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Worker
    {
        private Process _workerProcess;
        private ProcessStartInfo _workerProcessStartInfo;
        private readonly JsonSerializer _serializer;

        public Worker(ProcessStartInfo startInfo)
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

        public static void Run()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));

            Log("Worker created");

            var serializer = new JsonSerializer();
            while (true)
            {
                var reader = new JsonTextReader(Console.In);
                var message = serializer.Deserialize<Message>(reader);
                Log("Worker received {0} command with value {1}.", message.Command, message.Value);

                if (string.Equals(message.Command, "ping"))
                {
                    var response = new Message()
                    {
                        Command = "pong",
                        Value = message.Value
                    };

                    serializer.Serialize(new JsonTextWriter(Console.Out), response);
                    Log("Worker sent {0} command with value {1}.", response.Command, response.Value);
                }
            }
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
