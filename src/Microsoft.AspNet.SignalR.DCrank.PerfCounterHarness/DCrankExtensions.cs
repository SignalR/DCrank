using Newtonsoft.Json;
using Owin;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DCrankExtensions
    {
        public static IAppBuilder UseDCrankEndpoint(this IAppBuilder app)
        {
            return app.Map("/_dcrank", map =>
                {
                    map.Run(context =>
                        {
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(
                                new { DatabaseConnectionString = PerformanceCounterInformation.ConnectionString }));
                        });
                });
        }
    }
}
