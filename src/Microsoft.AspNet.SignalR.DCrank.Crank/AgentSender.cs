using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class AgentSender : IAgent
    {
        private readonly StreamWriter _outputStreamWriter;

        public AgentSender(StreamWriter outputStreamWriter)
        {
            _outputStreamWriter = outputStreamWriter;
        }

        public async Task Pong(int id, int value)
        {
            var parameters = new
            {
                Id = id,
                Value = value
            };

            await Send("pong", JToken.FromObject(parameters));
        }

        public async Task Log(int id, string text)
        {
            var parameters = new
            {
                Id = id,
                Text = text
            };

            await Send("log", JToken.FromObject(parameters));
        }

        public async Task Status(
            int id,
            StatusInformation statusInformation)
        {
            var parameters = new
            {
                Id = id,
                StatusInformation = statusInformation
            };

            await Send("status", JToken.FromObject(parameters)); ;
        }

        private async Task Send(string method, JToken parameters)
        {
            await _outputStreamWriter.WriteLineAsync(
                JsonConvert.SerializeObject(new Message()
                {
                    Command = method,
                    Value = parameters
                }));
            await _outputStreamWriter.FlushAsync();
        }
    }
}
