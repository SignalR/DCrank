
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

        public void AgentHeartbeat()
        {
            Clients.All.agentConnected(Context.ConnectionId);
        }

        public void WorkerHeartbeat(int workerId)
        {
            Clients.All.workerConnected(Context.ConnectionId, workerId);
        }

        public void StartWorker(string connectionId)
        {
            Clients.Client(connectionId).StartWorker();
        }

    }
}