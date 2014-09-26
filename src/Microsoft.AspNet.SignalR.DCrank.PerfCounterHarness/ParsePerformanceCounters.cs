using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class PerformanceCounterParser
    {
        public static List<DCrankPerformanceCounter> ReadCounters(List<PerformanceCounterSample> counterSamples)
        {
            if (counterSamples.Count < 2)
            {
                return null;
            }

            var counterList = new List<DCrankPerformanceCounter>();

            var latestCounterSample = counterSamples[0];
            var previousCounterSample = counterSamples[1];

            if (latestCounterSample != null && previousCounterSample != null)
            {
                var latestSampleCounterValues = JsonConvert.DeserializeObject<PerformanceCounterJsonDefinition>(latestCounterSample.PerformanceCounterJsonBlob);
                var previousSampleCounterValues = JsonConvert.DeserializeObject<PerformanceCounterJsonDefinition>(previousCounterSample.PerformanceCounterJsonBlob);

                // Creating dictionary to link perf counters to values
                var latestPerfCounterValueDictionary = latestSampleCounterValues.Values.ToDictionary<PerformanceCounterValues, string, long>
                (
                    counterValue => counterValue.ValueId,
                    counterValue => counterValue.Value
                );

                var previousPerfCounterValueDictionary = previousSampleCounterValues.Values.ToDictionary<PerformanceCounterValues, string, long>
                (
                    counterValue => counterValue.ValueId,
                    counterValue => counterValue.Value
                );

                foreach (var perfCounterDefinition in latestSampleCounterValues.Definitions)
                {
                    var perfCounter = new DCrankPerformanceCounter(perfCounterDefinition.Name);

                    if (perfCounterDefinition.Type == PerfCounterHarness.PerformanceCounterType.Total)
                    {
                        long value;
                        latestPerfCounterValueDictionary.TryGetValue(perfCounterDefinition.ValueId, out value);

                        perfCounter.RawValue = value;
                    }
                    else if (perfCounterDefinition.Type == PerfCounterHarness.PerformanceCounterType.PerSecRate)
                    {
                        long latestValue, previousValue;

                        if (latestPerfCounterValueDictionary.TryGetValue(perfCounterDefinition.ValueId, out latestValue)
                            && previousPerfCounterValueDictionary.TryGetValue(perfCounterDefinition.ValueId, out previousValue))
                        {
                            perfCounter.RawValue = (latestValue - previousValue) / latestCounterSample.Timestamp.Subtract(previousCounterSample.Timestamp).Seconds;
                        }
                    }

                    counterList.Add(perfCounter);
                }
            }

            return counterList;
        }
    }
}
