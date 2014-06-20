
using System.Threading.Tasks;
namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        private readonly string _uiGroup = "Dashboard";

        public override async Task OnConnected()
        {
            var isDashboard = Context.Request.QueryString["UI"] == "1";
            if (isDashboard)
            {
                await Groups.Add(Context.ConnectionId, _uiGroup);
            }
        }

        // A test to see if there is end to end communication - see if the int you send is actually received
        public void PingAgent(int value)
        {
            Clients.All.pingAgent(value);
        }

        public void PingWorker(int workerId, int value)
        {
            Clients.All.pingWorker(workerId, value);
        }

        public void PongAgent(int value)
        {
            Clients.Group(_uiGroup).agentPongResponse(Context.ConnectionId, value);
        }

        public void PongWorker(int workerId, int value)
        {
            Clients.Group(_uiGroup).workerPongResponse(Context.ConnectionId, workerId, value);
        }

        // Want to use this to recieve messages from the Agent - has to be called from the agent - A test of end to end connectivity
        public void AgentHeartbeat()
        {
            Clients.Group(_uiGroup).agentConnected(Context.ConnectionId);
        }

        public void WorkerHeartbeat(int workerId)
        {
            Clients.Group(_uiGroup).workerConnected(Context.ConnectionId, workerId);
        }

        // Starts a worker process via the ui on the given agent
        public void StartWorker(string connectionId)
        {
            Clients.Client(connectionId).StartWorker();
        }

    }
}