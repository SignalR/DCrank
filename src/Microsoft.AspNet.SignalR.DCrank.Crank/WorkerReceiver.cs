using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class WorkerReceiver
    {
        private readonly StreamReader _reader;
        private readonly IWorker _worker;
        private CancellationTokenSource _receiveMessageCts;

        public WorkerReceiver(StreamReader reader, IWorker worker)
        {
            _reader = reader;
            _worker = worker;
        }

        public void Start()
        {
            if (_receiveMessageCts != null)
            {
                _receiveMessageCts.Cancel();
            }

            _receiveMessageCts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_receiveMessageCts.Token.IsCancellationRequested)
                {
                    var messageString = await _reader.ReadLineAsync();
                    var message = JsonConvert.DeserializeObject<Message>(messageString);

                    switch (message.Command.ToLowerInvariant())
                    {
                        case "ping":
                            await _worker.Ping(
                                message.Value["Value"].ToObject<int>());
                            break;
                        case "connect":
                            await _worker.Connect(
                                message.Value["TargetAddress"].ToObject<string>(),
                                message.Value["NumberOfConnections"].ToObject<int>());
                            break;
                        case "starttest":
                            await _worker.StartTest(
                                message.Value["SendInterval"].ToObject<int>(),
                                message.Value["SendBytes"].ToObject<int>());
                            break;
                        case "stop":
                            await _worker.Stop();
                            break;
                    }
                }
            });
        }

        public void Stop()
        {
            _receiveMessageCts.Cancel();
        }

    }
}
