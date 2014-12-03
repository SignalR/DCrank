using System;
using System.Data.Entity;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DependencyResolverExtensions
    {
        private static readonly TimeSpan _defaultUpdateInterval = TimeSpan.FromSeconds(5);

        public static IDependencyResolver AddDCrankHarness(this IDependencyResolver resolver, string databaseConnectionString, TimeSpan? updateInterval = null)
        {
            if (updateInterval == null)
            {
                updateInterval = _defaultUpdateInterval;
            };

            Database.SetInitializer(new DropCreateDatabaseAlways<PerformanceCounterSampleContext>());

            var perfCounterManager = new DCrankPerformanceCounterManager();
            resolver.Register(typeof(IPerformanceCounterManager), () => perfCounterManager);

            var perfCounterConsumer = new PerformanceCounterConsumer(perfCounterManager, databaseConnectionString, updateInterval.Value);
            perfCounterConsumer.StartWritingPerformanceCounters();

            PerformanceCounterInformation.ConnectionString = databaseConnectionString;

            return resolver;
        }
    }
}
