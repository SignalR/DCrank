using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Linq;

namespace Microsoft.AspNet.SignalR.LoadTestHarness
{
    /// <summary>
    /// Summary description for ITimerManager
    /// </summary>
    public class TimerManager
    {
        private IHubConnectionContext<dynamic> _hubConnectionContext;
        private IConnection _persistantConnectionContext;
        private int _broadcastSize;

        public TimerManager(IConnectionManager connectionManager)
        {
            _hubConnectionContext = connectionManager.GetHubContext<Dashboard>().Clients;
            _persistantConnectionContext = connectionManager.GetConnectionContext<TestConnection>().Connection;

            BroadcastSize = 32;
            BroadcastCount = 1;
            BroadcastSeconds = 1;
            ActualFps = 0;

            TimerInstance = new Lazy<HighFrequencyTimer>(() =>
            {
                var clients = _hubConnectionContext;
                var connection = _persistantConnectionContext;
                return new HighFrequencyTimer(1,
                    _ =>
                    {
                        if (BatchingEnabled)
                        {
                            var count = BroadcastCount;
                            var payload = BroadcastPayload;
                            for (var i = 0; i < count; i++)
                            {
                                connection.Broadcast(payload);
                            }
                        }
                        else
                        {
                            connection.Broadcast(BroadcastPayload);
                        }
                    },
                    () => clients.All.started(),
                    () => { clients.All.stopped(); clients.All.serverFps(0); },
                    fps => { ActualFps = fps; clients.All.serverFps(fps); }
                    );
            });
        }

        public Lazy<HighFrequencyTimer> TimerInstance { get; private set; }

        public int BroadcastSize
        {
            get
            {
                return _broadcastSize;
            }
            set
            {
                _broadcastSize = value; BroadcastPayload = String.Join("", Enumerable.Range(0, BroadcastSize - 1).Select(i => "a"));
            }
        }

        public string BroadcastPayload { get; private set; }

        public int BroadcastCount { get; set; }

        public int BroadcastSeconds { get; set; }

        public bool BatchingEnabled { get; set; }

        public int ActualFps { get; set; }
    }
}