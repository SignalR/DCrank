using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerHub : Hub
    {
        private string _message = "has connected";
        private int _recieved;
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send()
        {
            // Call the broadcastMessage method to the UI with the appropriate message.
            string message = this.GetMessage();
            Clients.All.broadcastMessage(message);
        }

        public void AgentsAlive(int x)
        {
            Clients.All.ping(x);
        }

        public void Pong(int x)
        {
            _recieved = x;
            Clients.All.broadcastMessage(_recieved);

        }
        //Returns the message
        public string GetMessage()
        {
            string message = _message;
            return message;
        }

        //Want to use this to recieve messages from the Agent
        public void AgentHeartbeat(string connectionId)
        {
            _message = connectionId + _message;
            Send();
        }
    }
}