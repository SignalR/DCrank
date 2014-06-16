﻿
namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        private string _message = "";
        public void Hello()
        {
            Clients.All.hello();
        }

        // Call the broadcastMessage method to the UI with the appropriate message.
        public void Send()
        {
            Clients.All.broadcastMessage(_message + "has connected");
        }

        public void AgentsAlive(int x)
        {
            Clients.All.pong(x);
        }

        //A test to see if there is end to end communication - see if the int you send is actually received
        public void Ping(int x)
        {
            Clients.All.broadcastMessage(x);
        }

        //Allows you to broadcast to a specific client on the hub - future method not yet used
        public void RespondToClient(string conId)
        {
            GetClients(conId).Send();
        }

        //Establishes a way to identify a specific client on the hub - future method not yet used
        private dynamic GetClients(string conId = "")
        {
            var clients = GlobalHost.ConnectionManager.GetHubContext<ControllerHub>().Clients;
            if (conId == "")
            {
                return clients;
            }

            return clients.User(conId);
        }

        //Want to use this to recieve messages from the Agent - has to be called from the agent - A test of end to end connectivity
        public void AgentHeartbeat(string connectionId)
        {
            _message = connectionId;//The agent passes us their id when they connect so it is a neccessary parameter to establish unique identifiers
            Send();
        }
    }
}