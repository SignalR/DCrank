using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class PerformanceCounters
    {
        // Singleton instance
        private readonly static Lazy<PerformanceCounters> _performanceCounterInstance = new Lazy<PerformanceCounters>(() => new PerformanceCounters(GlobalHost.ConnectionManager.GetHubContext<ControllerHub>().Clients));

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);
        private Timer _timer;
        private DateTimeOffset _latestSampleTimestamp;
        private readonly object _updatePerformanceCounters = new object();
        private volatile bool _updatingPerformanceCounters = false;

        // Database as a list for the javascript
        private string _connectionString;

        private PerformanceCounters(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public static PerformanceCounters Instance
        {
            get
            {
                return _performanceCounterInstance.Value;
            }
        }

        public void Start(string connectionString)
        {
            _latestSampleTimestamp = DateTimeOffset.UtcNow;
            _connectionString = connectionString;

            TryReset();

            if (_timer == null)
            {
                _timer = new Timer(UpdatePerformanceCounters, null, _updateInterval, _updateInterval);
            }
        }

        private void TryReset()
        {
            try
            {
                using (var context = new PerformanceCounterSampleContext(_connectionString))
                {
                    context.Database.Delete();
                    context.Database.Create();
                }
            }
            catch (Exception ex)
            {
                // Trace?
            }
        }

        public void Stop()
        {
            //if (_timer != null)
            //{
            //    _timer.Dispose();
            //    _timer = null;
            //}
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        private void UpdatePerformanceCounters(object state)
        {
            if (!_updatingPerformanceCounters)
            {
                lock (_updatePerformanceCounters)
                {
                    try
                    {
                        _updatingPerformanceCounters = true;

                        using (var context = new PerformanceCounterSampleContext(_connectionString))
                        {
                            context.Database.CreateIfNotExists();

                            //if (_latestSampleTimestamp == default(DateTimeOffset))
                            //{
                            //    samples = context.PerformanceCounterSamples.OrderByDescending(s => s.PerformanceCounterSampleId).Take(2).ToList();
                            //}
                            //else
                            //{
                            var samples = (from sample in context.PerformanceCounterSamples
                                           where sample.Timestamp.CompareTo(_latestSampleTimestamp) >= 0
                                           select sample).OrderByDescending(s => s.PerformanceCounterSampleId).ToList();
                            //}

                            if (samples.Count > 0)
                            {
                                _latestSampleTimestamp = samples.First().Timestamp;
                            }

                            Clients.All.updatePerfCounters(PerformanceCounterParser.ReadCounters(samples), _latestSampleTimestamp);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Trace
                    }
                    finally
                    {
                        _updatingPerformanceCounters = false;
                    }
                }
            }
        }
    }
}