using System;
using System.Text;
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
                    map.Run(async context =>
                        {
                            if (context.Request.QueryString.HasValue && context.Request.QueryString.Value.Contains("error"))
                            {
                                var responseString = new StringBuilder();

                                foreach (var item in DCrankErrorList.ErrorList)
                                {
                                    responseString.Append(String.Format("{0} : {1} \n", item.Item1, item.Item2));
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
