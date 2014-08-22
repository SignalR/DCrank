﻿using Newtonsoft.Json;
using Owin;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DCrankExtensions
    {
        public static IAppBuilder UseDCrankEndpoint(this IAppBuilder app)
        {
            var endPoint = "/_dcrank";
            return app.Map(endPoint, map =>
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