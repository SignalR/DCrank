using System;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterSample
    {
        public int PerformanceCounterSampleId { get; set; }
        public DateTime Timestamp { get; set; }
        public long ConnectionMessagesReceivedPerSec { get; set; }
        public long ConnectionMessagesReceivedTotal { get; set; }
        public long ConnectionMessagesSentPerSec { get; set; }
        public long ConnectionMessagesSentTotal { get; set; }
        public long ConnectionsConnected { get; set; }
        public long ConnectionsCurrent { get; set; }
        public long ConnectionsDisconnected { get; set; }
        public long ConnectionsReconnected { get; set; }
    }
}
