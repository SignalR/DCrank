using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterJsonDefinition
    {
        public List<PerformanceCounterDefinition> Definitions { get; set; }
        public List<PerformanceCounterValues> Values { get; set; }
    }
}
