using System;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class ServiceCollectionExtensions
    {
        private static readonly TimeSpan _defaultUpdateInterval = TimeSpan.FromSeconds(5);

        public static ServiceCollection AddDCrankHarness(this ServiceCollection services, string databaseConnectionString, TimeSpan? updateInterval = null)
        {
            if (updateInterval == null)
            {
                updateInterval = _defaultUpdateInterval;
            };

            PerformanceCounterInformation.ConnectionString = databaseConnectionString;
            using (var context = new PerformanceCounterSampleContext(databaseConnectionString))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            var performanceCounterManager = new DCrankPerformanceCounterManager();
            services.AddInstance<IPerformanceCounterManager>(performanceCounterManager);

            var performanceCounterConsumer = new PerformanceCounterConsumer(performanceCounterManager, databaseConnectionString, updateInterval.Value);
            services.AddInstance<PerformanceCounterConsumer>(performanceCounterConsumer);
            performanceCounterConsumer.StartWritingPerformanceCounters();            

            return services;
        }
    }
}