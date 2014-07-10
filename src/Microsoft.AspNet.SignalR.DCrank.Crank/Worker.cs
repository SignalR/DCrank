using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Worker
    {
        public static void Run()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));

            //Log("Worker created");

            var serializer = new JsonSerializer();
            while (true)
            {
                var reader = new JsonTextReader(Console.In);
                var message = serializer.Deserialize<Message>(reader);
                //Log("Worker received {0} command with value {1}.", message.Command, message.Value);

                if (string.Equals(message.Command, "ping"))
                {
                    var response = new Message()
                    {
                        Command = "pong",
                        Value = message.Value
                    };

                    serializer.Serialize(new JsonTextWriter(Console.Out), response);
                    //Log("Worker sent {0} command with value {1}.", response.Command, response.Value);
                }
            }
        }
    }
}
