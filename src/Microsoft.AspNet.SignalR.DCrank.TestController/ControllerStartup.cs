using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Microsoft.AspNet.SignalR.DCrank.TestController.ControllerStartup))]

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class ControllerStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HubConfiguration { EnableDetailedErrors = true };
            app.MapSignalR(config);
        }
    }
}
