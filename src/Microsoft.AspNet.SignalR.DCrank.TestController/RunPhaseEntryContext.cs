using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class RunPhaseEntryContext : DbContext
    {
        public DbSet<RunPhaseEntry> RunPhaseEntries { get; set; }

        public RunPhaseEntryContext(string connectionString)
            : base(connectionString)
        { }
    }
}