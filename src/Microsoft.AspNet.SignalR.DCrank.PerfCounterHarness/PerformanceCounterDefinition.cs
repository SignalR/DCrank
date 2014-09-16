using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterDefinition
    {
        public string Name { get; set; }
        public string ValueId { get; set; }
        public PerformanceCounterType Type { get; set; }
    }
}
