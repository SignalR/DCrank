using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DependencyResolverExtensions
    {
        public static IDependencyResolver AddDCrankHarness(this IDependencyResolver resolver, string databaseConnectionString, int updateInterval = 5)
        {
            var perfCounterManager = new PerformanceCounterManagerDCrank(databaseConnectionString, updateInterval);
            resolver.Register(typeof(IPerformanceCounterManager), () => perfCounterManager);

            PerformanceCounterInformation.ConnectionString = databaseConnectionString;
            Database.SetInitializer(new DropCreateDatabaseAlways<PerformanceCounterSampleContext>());

            return resolver;
        }
    }
}
