using System.Data.Entity;
using System.Data.SqlClient;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterSampleContext : DbContext
    {
        static PerformanceCounterSampleContext()
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<PerformanceCounterSampleContext>());
        }

        public DbSet<PerformanceCounterSample> PerformanceCounterSamples { get; set; }

        public PerformanceCounterSampleContext(string connectionString) :
            base(connectionString)
        {
        }
    }
}
