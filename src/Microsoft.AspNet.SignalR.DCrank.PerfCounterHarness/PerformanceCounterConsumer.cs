using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Data.Entity.Update;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterConsumer
    {
        private readonly IPerformanceCounterManager _perfCounterManager;
        private readonly string _connectionString;

        public PerformanceCounterConsumer(IPerformanceCounterManager perfCounterManager, string connectionString, TimeSpan updateInterval)
        {
            _perfCounterManager = perfCounterManager;
            _connectionString = connectionString;

            Task.Run(() => UpdatePerformanceCounters(perfCounterManager, connectionString, updateInterval));
        }

        private void UpdatePerformanceCounters(IPerformanceCounterManager perfCounterManager, string connectionString, TimeSpan updateInterval)
        {
            while (true)
            {
                using (var database = new PerformanceCounterSampleContext(connectionString))
                {
                    try
                    {
                        var definitionList = new List<PerformanceCounterDefinition>();
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionMessagesSentPerSec", Type = PerformanceCounterType.PerSecRate, ValueId = 0 });
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionMessagesReceivedTotal", Type = PerformanceCounterType.Total, ValueId = 1 });
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionMessagesSentTotal", Type = PerformanceCounterType.Total, ValueId = 0 });
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionsConnected", Type = PerformanceCounterType.Total, ValueId = 2 });
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionsCurrent", Type = PerformanceCounterType.Total, ValueId = 3 });
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionsReconnected", Type = PerformanceCounterType.Total, ValueId = 4 });
                        definitionList.Add(new PerformanceCounterDefinition { Name = "ConnectionsDisconnected", Type = PerformanceCounterType.Total, ValueId = 5 });

                        var valueList = new List<PerformanceCounterValues>();
                        valueList.Add(new PerformanceCounterValues() { ValueId = 0, Value = _perfCounterManager.ConnectionMessagesSentTotal.RawValue });
                        valueList.Add(new PerformanceCounterValues() { ValueId = 1, Value = _perfCounterManager.ConnectionMessagesReceivedTotal.RawValue });
                        valueList.Add(new PerformanceCounterValues() { ValueId = 2, Value = _perfCounterManager.ConnectionsConnected.RawValue });
                        valueList.Add(new PerformanceCounterValues() { ValueId = 3, Value = _perfCounterManager.ConnectionsCurrent.RawValue });
                        valueList.Add(new PerformanceCounterValues() { ValueId = 4, Value = _perfCounterManager.ConnectionsReconnected.RawValue });
                        valueList.Add(new PerformanceCounterValues() { ValueId = 5, Value = _perfCounterManager.ConnectionsDisconnected.RawValue });

                        var perfCounterJsonValue = JsonConvert.SerializeObject(new
                        {
                            definitions = definitionList,
                            values = valueList
                        });

                        var perfCounterSample = new PerformanceCounterSample
                        {
                            Timestamp = DateTime.UtcNow,
                            PerformanceCounterJsonBlob = perfCounterJsonValue
                        };

                        database.PerformanceCounterSamples.Add(perfCounterSample);
                        database.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        Debug.WriteLine(ex.InnerException.Message);
                    }
                }

                Thread.Sleep(updateInterval);
            }
        }
    }
}
