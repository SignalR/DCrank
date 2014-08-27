using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterManagerDCrank : IPerformanceCounterManager
    {
        // Only for now to avoid initializing all the counters
        private IPerformanceCounter _testValue;

        public PerformanceCounterManagerDCrank()
        {
            ConnectionMessagesReceivedPerSec = new DCrankPerformanceCounter("ConnectionMessagesReceivedPerSec", DCrankPerformanceCounterType.PerSecRate);
            ConnectionMessagesReceivedTotal = new DCrankPerformanceCounter("ConnectionMessagesReceivedTotal", DCrankPerformanceCounterType.Total);
            ConnectionMessagesSentPerSec = new DCrankPerformanceCounter("ConnectionMessagesSentPerSec", DCrankPerformanceCounterType.PerSecRate);
            ConnectionMessagesSentTotal = new DCrankPerformanceCounter("ConnectionMessagesSentTotal", DCrankPerformanceCounterType.Total);
            ConnectionsConnected = new DCrankPerformanceCounter("ConnectionsConnected", DCrankPerformanceCounterType.Total);
            ConnectionsCurrent = new DCrankPerformanceCounter("ConnectionsCurrent", DCrankPerformanceCounterType.Total);
            ConnectionsDisconnected = new DCrankPerformanceCounter("ConnectionsDisconnected", DCrankPerformanceCounterType.Total);
            ConnectionsReconnected = new DCrankPerformanceCounter("ConnectionsReconnected", DCrankPerformanceCounterType.Total);

            _testValue = new DCrankPerformanceCounter("TestValue", DCrankPerformanceCounterType.Total);
        }

        public IPerformanceCounter ConnectionMessagesReceivedPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionMessagesReceivedTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionMessagesSentPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionMessagesSentTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionsConnected
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionsCurrent
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionsDisconnected
        {
            get;
            private set;
        }

        public IPerformanceCounter ConnectionsReconnected
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsAllPerSec
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsAllTotal
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsHubInvocationPerSec
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsHubInvocationTotal
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsHubResolutionPerSec
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsHubResolutionTotal
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsTransportPerSec
        {
            get { return _testValue; }

        }

        public IPerformanceCounter ErrorsTransportTotal
        {
            get { return _testValue; }

        }

        public void Initialize(string instanceName, CancellationToken hostShutdownToken)
        {

        }

        public IPerformanceCounter LoadCounter(string categoryName, string counterName, string instanceName, bool isReadOnly)
        {
            return null;
        }

        public IPerformanceCounter MessageBusAllocatedWorkers
        {
            get { return _testValue; }

        }

        public IPerformanceCounter MessageBusBusyWorkers
        {
            get { return _testValue; }

        }

        public IPerformanceCounter MessageBusMessagesPublishedPerSec
        {
            get { return _testValue; }

        }

        public IPerformanceCounter MessageBusMessagesPublishedTotal
        {
            get { return _testValue; }

        }

        public IPerformanceCounter MessageBusMessagesReceivedPerSec
        {
            get { return _testValue; }

        }

        public IPerformanceCounter MessageBusMessagesReceivedTotal
        {
            get { return _testValue; }

        }

        public IPerformanceCounter MessageBusSubscribersCurrent
        {
            get { return _testValue; }
        }

        public IPerformanceCounter MessageBusSubscribersPerSec
        {
            get { return _testValue; }
        }

        public IPerformanceCounter MessageBusSubscribersTotal
        {
            get { return _testValue; }
        }

        public IPerformanceCounter MessageBusTopicsCurrent
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutErrorsPerSec
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutErrorsTotal
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutMessageBusMessagesReceivedPerSec
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutSendQueueLength
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutStreamCountBuffering
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutStreamCountOpen
        {
            get { return _testValue; }
        }

        public IPerformanceCounter ScaleoutStreamCountTotal
        {
            get { return _testValue; }
        }
    }
}
