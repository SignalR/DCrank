
namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterDefinition
    {
        public string Name { get; set; }
        public int ValueId { get; set; }
        public PerformanceCounterType Type { get; set; }
    }
}