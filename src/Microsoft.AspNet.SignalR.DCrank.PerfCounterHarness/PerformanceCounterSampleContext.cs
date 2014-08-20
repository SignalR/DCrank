using System.Data.Entity;
using System.Data.SqlClient;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterSampleContext : DbContext
    {
        public DbSet<PerformanceCounterSample> PerformanceCounterSamples { get; set; }

        public PerformanceCounterSampleContext(string connectionString) :
            base(connectionString)
        {
        }
    }
}
