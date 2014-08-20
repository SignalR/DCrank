using System;
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
        private readonly static Lazy<PerformanceCounters> _instance = new Lazy<PerformanceCounters>(() => new PerformanceCounters(GlobalHost.ConnectionManager.GetHubContext<ControllerHub>().Clients));
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(5000);
        private Timer _timer;

        // Database as a list for the javascript
        private PerformanceCounterSample _latestUpdate;
        private string _connectionString;

        private PerformanceCounters(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public static PerformanceCounters Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void Start(string connectionString)
        {
            _connectionString = connectionString;
            _timer = new Timer(UpdatePerformanceCounters, null, _updateInterval, _updateInterval);
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        private void UpdatePerformanceCounters(object state)
        {
            using (var database = new PerformanceCounterSampleContext(_connectionString))
            {
                try
                {
                    var currentData = database.PerformanceCounterSamples.LastOrDefault();
                    if (currentData != null && currentData != _latestUpdate)
                    {
                        _latestUpdate = currentData;
                        Clients.All.updatePerfCounters(_latestUpdate);
                    }

                }
                catch (DbUpdateException ex)
                {
                    Debug.WriteLine(ex.InnerException.Message);
                }
            }
        }
    }
}