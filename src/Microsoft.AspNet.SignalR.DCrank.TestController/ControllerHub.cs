
using System.Threading.Tasks;
namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {

        public override async Task OnConnected()
        {
            var isUI = Context.Request.QueryString["UI"]=="1";
            if (isUI)
            {
                await Groups.Add(Context.ConnectionId, "UIIdentifier");
            }
        }

        //A test to see if there is end to end communication - see if the int you send is actually received
        public void Ping(int value)
        {
            Clients.All.pong(value);
        }

        public void AgentAlive(int value)
        {
            Clients.Group("UIIdentifier").pongResponse(Context.ConnectionId, value);

        }

        //Want to use this to recieve messages from the Agent - has to be called from the agent - A test of end to end connectivity
        public void AgentHeartbeat()
        {
            Clients.Group("UIIdentifier").agentConnected(Context.ConnectionId);
        }
    }
}