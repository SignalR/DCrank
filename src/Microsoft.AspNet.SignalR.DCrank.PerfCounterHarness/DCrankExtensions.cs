using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DCrankExtensions
    {
        public static IBuilder UseDCrankEndpoint(this IBuilder app)
        {
            return app.Map("/_dcrank", map =>
                {
                    map.Run(async context =>
                        {
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                new { DatabaseConnectionString = PerformanceCounterInformation.ConnectionString }));
                        });
                });
        }
    }
}
