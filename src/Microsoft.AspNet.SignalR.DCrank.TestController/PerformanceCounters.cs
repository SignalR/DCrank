using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
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
        private readonly object _updatePerformanceCounters = new object();
        private volatile bool _updatingPerformanceCounters = false;

        // Database as a list for the javascript
        private PerformanceCounterSample _lastSample;
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
            _connectionString = connectionString;
            if (_timer == null)
            {
                _timer = new Timer(UpdatePerformanceCounters, null, _updateInterval, _updateInterval);
            }
        }

        public void Stop()
        {
            _timer.Dispose();
            _timer = null;
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
                    _updatingPerformanceCounters = true;

                    using (var database = new PerformanceCounterSampleContext(_connectionString))
                    {
                        var currentData = database.PerformanceCounterSamples.OrderByDescending(s => s.PerformanceCounterSampleId).FirstOrDefault();

                        if (currentData != null && currentData != _lastSample)
                        {
                            _lastSample = currentData;
                            Clients.All.updatePerfCounters(_lastSample);
                        }
                    }

                    _updatingPerformanceCounters = false;
                }
            }
        }
    }
}