using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class ControllerClient
    {
        private const string _hubName = "ControllerHub";
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public ControllerClient(string url)
        {
            _connection = new HubConnection(url);
            _proxy = _connection.CreateHubProxy(_hubName);
        }

        public void Initialize()
        {
            _connection.StateChanged +=
                stateChange =>
                {
                    if (stateChange.NewState == ConnectionState.Connected)
                    {
                        _proxy.Invoke("AgentHeartbeat");
                    }
                };

            _proxy.On<int>("ping", value =>
            {
                _proxy.Invoke("pong", value);
            });
        }

        public async Task Start()
        {
            await _connection.Start();
        }
    }
}
