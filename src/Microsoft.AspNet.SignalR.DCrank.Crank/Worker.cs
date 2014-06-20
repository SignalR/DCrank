using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Worker
    {
        public Worker() { }

        public async Task Run()
        {
            await Task.Factory.StartNew(() =>
            {
                var serializer = new JsonSerializer();
                while (true)
                {
                    var message = serializer.Deserialize<Message>(new JsonTextReader(Console.In));
                    if (string.Equals(message.Command, "ping"))
                    {
                        serializer.Serialize(new JsonTextWriter(Console.Out), new Message(){ Command = "pong", Value = message.Value });
                    }
                }
            });
        }
    }
}
