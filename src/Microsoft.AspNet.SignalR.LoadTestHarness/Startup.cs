using System;
using Microsoft.AspNet.SignalR.LoadTestHarness;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness;
using Owin;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Startup))]

namespace Microsoft.AspNet.SignalR.LoadTestHarness
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PerformanceCounters"];
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(60);
            GlobalHost.DependencyResolver.AddDCrankHarness(connectionString.ConnectionString);

            app.MapSignalR<TestConnection>("/TestConnection");
            app.MapSignalR();

            app.UseDCrankEndpoint();

            app.Map("/kill", map =>
            {
                map.Run(ctx =>
                {
                    Process.GetCurrentProcess().Kill();
                    return Task.FromResult(true);
                });
            });

            Dashboard.Init();
        }
    }
}