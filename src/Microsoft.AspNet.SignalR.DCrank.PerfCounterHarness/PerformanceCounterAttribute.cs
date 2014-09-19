using System;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterAttribute : Attribute
    {
        public string BaseCounterName { get; set; }
        public PerformanceCounterType CounterType { get; set; }
    }
}