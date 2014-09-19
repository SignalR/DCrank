using System;
using System.Text;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DCrankExtensions
    {
        public static IApplicationBuilder UseDCrankEndpoint(this IApplicationBuilder app)
        {
            return app.Map("/_dcrank", map =>
                {
                    map.Run(async context =>
                        {
                            if (context.Request.Query["info"] != null && context.Request.Query["info"].Contains("error"))
                            {
                                var responseString = new StringBuilder();

                                foreach (var item in DCrankErrorList.ErrorList)
                                {
                                    responseString.AppendFormat("{0} : {1} {2}", item.Item1, item.Item2, Environment.NewLine);
                                }

                                await context.Response.WriteAsync(responseString.ToString());
                            }
                            else
                            {
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new { DatabaseConnectionString = PerformanceCounterInformation.ConnectionString }));
                            }
                        });
                });
        }
    }
}
