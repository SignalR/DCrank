using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Client
    {
        private Connection _connection;

        public Action OnClosed;
        public Action<string> OnMessage;

        public async Task CreateConnection(CrankArguments arguments)
        {
            _connection = new Connection(arguments.Url + "TestConnection");

            _connection.Received += OnMessage;
            _connection.Closed += OnClosed;

            int connectCount;

            for (connectCount = 0; connectCount < 3; connectCount++)
            {
                try
                {
                    if (arguments.Transport == null)
                    {
                        await _connection.Start();
                    }
                    else
                    {
                        await _connection.Start(arguments.GetTransport());
                    }

                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connection.Start Failed: {0}: {1}", e.GetType(), e.Message);
                }
            }

            if (connectCount == 3)
            {
                // Send information back to the UI?
            }
        }

        public async Task StartTest(CrankArguments arguments)
        {
            var payload = (arguments.SendBytes == 0) ? String.Empty : new string('a', arguments.SendBytes);

            if (!String.IsNullOrEmpty(payload))
            {
                while (true)
                {
                    await _connection.Send(payload);

                    await Task.Delay(arguments.SendInterval);
                }
            }
        }

        public void StopConnection()
        {
            _connection.Stop();
        }
    }
}
