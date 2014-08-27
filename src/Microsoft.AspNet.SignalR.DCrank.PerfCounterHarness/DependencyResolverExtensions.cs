using System;
using System.Data.Entity;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DependencyResolverExtensions
    {
        private static TimeSpan _defaultUpdateInterval = TimeSpan.FromSeconds(5);
        public static IDependencyResolver AddDCrankHarness(this IDependencyResolver resolver, string databaseConnectionString, TimeSpan? updateInterval = null)
        {
            if (updateInterval == null)
            {
                updateInterval = TimeSpan.FromSeconds(5);
            };

            var perfCounterManager = new PerformanceCounterManagerDCrank();
            resolver.Register(typeof(IPerformanceCounterManager), () => perfCounterManager);

            var perfCounterConsumer = new PerformanceCounterConsumer(perfCounterManager, databaseConnectionString, updateInterval.Value);

            PerformanceCounterInformation.ConnectionString = databaseConnectionString;
            Database.SetInitializer(new DropCreateDatabaseAlways<PerformanceCounterSampleContext>());

            return resolver;
        }
    }
}
