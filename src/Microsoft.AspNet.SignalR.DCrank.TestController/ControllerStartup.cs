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
            app.MapSignalR();

            //GlobalHost.DependencyResolver.AddDCrankHarness("Data Source=WIN-7NI482MOOEG;Initial Catalog=test1;User ID=sa;Password=qwerty12@;MultipleActiveResultSets=True", 5);
        }
    }
}
