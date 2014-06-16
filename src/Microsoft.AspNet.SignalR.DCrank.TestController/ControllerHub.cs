using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        private string _message = "has connected";
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send()
        {
            // Call the broadcastMessage method to the UI with the appropriate message.
            var message = GetMessage();
            Clients.All.broadcastMessage(message);
        }

        public void AgentsAlive(int x)
        {
            Clients.All.ping(x);
        }
        //A test to see if there is end to end communication - see if the int you send is actually received
        public void Pong(int x)
        {
            Clients.All.broadcastMessage(x);
        }
        //Returns the message to the UI
        public string GetMessage()
        {
            return _message;
        }

        //Want to use this to recieve messages from the Agent - has to be called from the agent - A test of end to end connectivity
        public void AgentHeartbeat(string connectionId)
        {
            _message = connectionId + _message;
            Send();
        }
    }
}