using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.SignalR.LoadTestHarness
{
    public class TestConnection : PersistentConnection
    {
        internal static ConnectionBehavior Behavior { get; set; }

        protected override Task OnReceived(HttpRequest request, string connectionId, string data)
        {
            if (Behavior == ConnectionBehavior.Echo)
            {
                Connection.Send(connectionId, data);
            }
            else if (Behavior == ConnectionBehavior.Broadcast)
            {
                Connection.Broadcast(data);
            }
            return Task.FromResult<object>(null);
        }
    }

    public enum ConnectionBehavior
    {
        ListenOnly,
        Echo,
        Broadcast
    }
}