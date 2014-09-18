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
                        var definitionList = new List<PerformanceCounterDefinition>()
                        {
                          new PerformanceCounterDefinition() { Name = "ConnectionMessagesSentPerSec", Type = PerformanceCounterType.PerSecRate, ValueId = 0 },
                          new PerformanceCounterDefinition() { Name = "ConnectionMessagesSentTotal", Type = PerformanceCounterType.Total, ValueId = 0 },

                          new PerformanceCounterDefinition() { Name = "ConnectionMessagesReceivedPerSec", Type = PerformanceCounterType.PerSecRate, ValueId = 1 },
                          new PerformanceCounterDefinition() { Name = "ConnectionMessagesReceivedTotal", Type = PerformanceCounterType.Total, ValueId = 1 },

                          new PerformanceCounterDefinition() { Name = "ConnectionsConnected", Type = PerformanceCounterType.Total, ValueId = 2 },
                          new PerformanceCounterDefinition() { Name = "ConnectionsCurrent", Type = PerformanceCounterType.Total, ValueId = 3 },
                          new PerformanceCounterDefinition() { Name = "ConnectionsReconnected", Type = PerformanceCounterType.Total, ValueId = 4 },
                          new PerformanceCounterDefinition() { Name = "ConnectionsDisconnected", Type = PerformanceCounterType.Total, ValueId = 5 },

                          new PerformanceCounterDefinition() { Name = "ErrorsAllPerSec", Type = PerformanceCounterType.PerSecRate, ValueId = 6 },
                          new PerformanceCounterDefinition() { Name = "ErrorsAllTotal", Type = PerformanceCounterType.Total, ValueId = 6 },

                          new PerformanceCounterDefinition() { Name = "ErrorsHubInvocationPerSec", Type = PerformanceCounterType.PerSecRate, ValueId = 7 },
                          new PerformanceCounterDefinition() { Name = "ErrorsHubInvocationTotal", Type = PerformanceCounterType.Total, ValueId = 7 },

                          new PerformanceCounterDefinition() { Name = "ErrorsHubTransportPerSec", Type = PerformanceCounterType.PerSecRate, ValueId = 8 },
                          new PerformanceCounterDefinition() { Name = "ErrorsHubTransportTotal", Type = PerformanceCounterType.Total, ValueId = 8 }
                        };

                        var valueList = new List<PerformanceCounterValues>()
                        {
                            new PerformanceCounterValues() { ValueId = 0, Value = _perfCounterManager.ConnectionMessagesSentTotal.RawValue },
                            new PerformanceCounterValues() { ValueId = 1, Value = _perfCounterManager.ConnectionMessagesReceivedTotal.RawValue },
                            new PerformanceCounterValues() { ValueId = 2, Value = _perfCounterManager.ConnectionsConnected.RawValue },
                            new PerformanceCounterValues() { ValueId = 3, Value = _perfCounterManager.ConnectionsCurrent.RawValue },
                            new PerformanceCounterValues() { ValueId = 4, Value = _perfCounterManager.ConnectionsReconnected.RawValue },
                            new PerformanceCounterValues() { ValueId = 5, Value = _perfCounterManager.ConnectionsDisconnected.RawValue },

                            new PerformanceCounterValues() { ValueId = 6, Value = _perfCounterManager.ErrorsAllTotal.RawValue },
                            new PerformanceCounterValues() { ValueId = 7, Value = _perfCounterManager.ErrorsHubInvocationTotal.RawValue },
                            new PerformanceCounterValues() { ValueId = 8, Value = _perfCounterManager.ErrorsTransportTotal.RawValue },
                        };

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
