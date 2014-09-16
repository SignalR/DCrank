using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterAttribute : System.Attribute
    {
        public string BaseCounterName { get; set; }
        public PerformanceCounterType CounterType { get; set; }
    }
}
