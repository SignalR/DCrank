using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterManagerDCrank : Microsoft.AspNet.SignalR.Infrastructure.PerformanceCounterManager
    {
        public PerformanceCounterManagerDCrank(string connectionString, int updateInterval)
        {
            Task.Run(() =>
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
                                ConnectionMessagesReceivedPerSec = ConnectionMessagesSentPerSec.RawValue,
                                ConnectionMessagesReceivedTotal = ConnectionMessagesReceivedTotal.RawValue,
                                ConnectionMessagesSentPerSec = ConnectionMessagesSentPerSec.RawValue,
                                ConnectionMessagesSentTotal = ConnectionMessagesSentTotal.RawValue,
                                ConnectionsConnected = ConnectionsConnected.RawValue,
                                ConnectionsCurrent = ConnectionsCurrent.RawValue,
                                ConnectionsReconnected = ConnectionsReconnected.RawValue,
                                ConnectionsDisconnected = ConnectionsDisconnected.RawValue
                            };

                            database.PerformanceCounterSamples.Add(perfCounterSample);
                            database.SaveChanges();
                        }
                        catch (DbUpdateException ex)
                        {
                            Debug.WriteLine(ex.InnerException.Message);
                        }
                    }

                    Thread.Sleep(updateInterval * 1000);
                }
            });
        }
    }
}
