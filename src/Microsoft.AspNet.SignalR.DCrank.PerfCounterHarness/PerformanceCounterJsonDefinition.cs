using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterJsonDefinition
    {
        public List<PerformanceCounterDefinition> Definitions { get; set; }
        public List<PerformanceCounterValues> Values { get; set; }
    }
}
