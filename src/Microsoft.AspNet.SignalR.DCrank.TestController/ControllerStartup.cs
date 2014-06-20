using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Microsoft.AspNet.SignalR.DCrank.TestController.ControllerStartup))]

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    // You can name this class anything as long as it contains the words 'Startup'
    public class ControllerStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
