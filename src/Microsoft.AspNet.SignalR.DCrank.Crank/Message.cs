using Newtonsoft.Json.Linq;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class Message
    {
        public string Command { get; set; }

        public JToken Value { get; set; }
    }
}
