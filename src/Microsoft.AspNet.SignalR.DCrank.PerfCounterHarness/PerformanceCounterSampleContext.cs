using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System.Data.SqlClient;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterSampleContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<PerformanceCounterSample> PerformanceCounterSamples { get; set; }

        public PerformanceCounterSampleContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PerformanceCounterSample>().ToTable("PerformanceCounterSamples");
        }

        protected override void OnConfiguring(DbContextOptions options)
        {
            options.UseSqlServer(_connectionString);
            base.OnConfiguring(options);
        }
    }
}