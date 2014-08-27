using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;

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
                        var perfCounterSample = new PerformanceCounterSample
                        {
                            Timestamp = DateTime.UtcNow,
                            ConnectionMessagesReceivedPerSec = _perfCounterManager.ConnectionMessagesSentPerSec.RawValue,
                            ConnectionMessagesReceivedTotal = _perfCounterManager.ConnectionMessagesReceivedTotal.RawValue,
                            ConnectionMessagesSentPerSec = _perfCounterManager.ConnectionMessagesSentPerSec.RawValue,
                            ConnectionMessagesSentTotal = _perfCounterManager.ConnectionMessagesSentTotal.RawValue,
                            ConnectionsConnected = _perfCounterManager.ConnectionsConnected.RawValue,
                            ConnectionsCurrent = _perfCounterManager.ConnectionsCurrent.RawValue,
                            ConnectionsReconnected = _perfCounterManager.ConnectionsReconnected.RawValue,
                            ConnectionsDisconnected = _perfCounterManager.ConnectionsDisconnected.RawValue
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
