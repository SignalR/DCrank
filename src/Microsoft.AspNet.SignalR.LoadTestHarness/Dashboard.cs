using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace Microsoft.AspNet.SignalR.LoadTestHarness
{
    public class Dashboard : Hub
    {
        private TimerManager _timerManager;

        public Dashboard(TimerManager timerManager )
        {
            _timerManager = timerManager;
        }

        private HighFrequencyTimer _timer { get { return _timerManager.TimerInstance.Value; } }

        public dynamic GetStatus()
        {
            return new
            {
                ConnectionBehavior = TestConnection.Behavior,
                BroadcastBatching = _timerManager.BatchingEnabled,
                BroadcastCount = _timerManager.BroadcastCount,
                BroadcastSeconds = _timerManager.BroadcastSeconds,
                BroadcastSize = _timerManager.BroadcastSize,
                Broadcasting = _timer.IsRunning(),
                ServerFps = _timerManager.ActualFps
            };
        }

        public void SetConnectionBehavior(ConnectionBehavior behavior)
        {
            TestConnection.Behavior = behavior;
            Clients.Others.connectionBehaviorChanged(((int)behavior).ToString());
        }

        public void SetBroadcastBehavior(bool batchingEnabled)
        {
            _timerManager.BatchingEnabled = batchingEnabled;
            Clients.Others.broadcastBehaviorChanged(batchingEnabled);
        }

        public void SetBroadcastRate(int count, int seconds)
        {
            // Need to turn the count/seconds into FPS
            _timerManager.BroadcastCount = count;
            _timerManager.BroadcastSeconds = seconds;
            _timer.FPS = _timerManager.BatchingEnabled ? 1 / (double)seconds : count;
            Clients.Others.broadcastRateChanged(count, seconds);
        }

        public void SetBroadcastSize(int size)
        {
            _timerManager.BroadcastSize = size;
            Clients.Others.broadcastSizeChanged(size.ToString());
        }

        public void ForceGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void StartBroadcast()
        {
            _timer.Start();
        }

        public void StopBroadcast()
        {
            _timer.Stop();
        }
    }
}
