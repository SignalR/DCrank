using System;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterSample
    {
        public int PerformanceCounterSampleId { get; set; }
        public string PerformanceCounterJsonBlob { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}