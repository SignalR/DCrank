using Microsoft.AspNet.SignalR.Infrastructure;
using System.Threading;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class DCrankPerformanceCounterManager : IPerformanceCounterManager
    {
        public DCrankPerformanceCounterManager(string connectionString, int updateInterval)
        {
            ConnectionMessagesReceivedPerSec = new DCrankPerformanceCounter("ConnectionMessagesReceivedPerSec");
            ConnectionMessagesReceivedTotal = new DCrankPerformanceCounter("ConnectionMessagesReceivedTotal");
            ConnectionMessagesSentPerSec = new DCrankPerformanceCounter("ConnectionMessagesSentPerSec");
            ConnectionMessagesSentTotal = new DCrankPerformanceCounter("ConnectionMessagesSentTotal");
            ConnectionsConnected = new DCrankPerformanceCounter("ConnectionsConnected");
            ConnectionsCurrent = new DCrankPerformanceCounter("ConnectionsCurrent");
            ConnectionsDisconnected = new DCrankPerformanceCounter("ConnectionsDisconnected");
            ConnectionsReconnected = new DCrankPerformanceCounter("ConnectionsReconnected");

            ErrorsAllPerSec = new DCrankPerformanceCounter("ErrorsAllPerSec");
            ErrorsAllTotal = new DCrankPerformanceCounter("ErrorsAllTotal");
            ErrorsHubInvocationPerSec = new DCrankPerformanceCounter("ErrorsHubInvocationPerSec");
            ErrorsHubInvocationTotal = new DCrankPerformanceCounter("ErrorsHubInvocationTotal");

            ErrorsHubResolutionPerSec = new DCrankPerformanceCounter("ErrorsHubResolutionPerSec");
            ErrorsHubResolutionTotal = new DCrankPerformanceCounter("ErrorsHubResolutionTotal");
            ErrorsTransportPerSec = new DCrankPerformanceCounter("ErrorsTransportPerSec");
            ErrorsTransportTotal = new DCrankPerformanceCounter("ErrorsTransportTotal");

            MessageBusAllocatedWorkers = new DCrankPerformanceCounter("MessageBusAllocatedWorkers");
            MessageBusBusyWorkers = new DCrankPerformanceCounter("MessageBusBusyWorkers");
            MessageBusMessagesPublishedPerSec = new DCrankPerformanceCounter("MessageBusMessagesPublishedPerSec");
            MessageBusMessagesPublishedTotal = new DCrankPerformanceCounter("MessageBusMessagesPublishedTotal");
            MessageBusMessagesReceivedPerSec = new DCrankPerformanceCounter("MessageBusMessagesReceivedPerSec");

            MessageBusMessagesReceivedTotal = new DCrankPerformanceCounter("MessageBusMessagesReceivedTotal");
            MessageBusSubscribersCurrent = new DCrankPerformanceCounter("MessageBusSubscribersCurrent");
            MessageBusSubscribersPerSec = new DCrankPerformanceCounter("MessageBusSubscribersPerSec");
            MessageBusSubscribersTotal = new DCrankPerformanceCounter("MessageBusSubscribersTotal");
            MessageBusTopicsCurrent = new DCrankPerformanceCounter("MessageBusTopicsCurrent");
            ScaleoutErrorsPerSec = new DCrankPerformanceCounter("ScaleoutErrorsPerSec");
            ScaleoutErrorsTotal = new DCrankPerformanceCounter("ScaleoutErrorsTotal");
            ScaleoutMessageBusMessagesReceivedPerSec = new DCrankPerformanceCounter("ScaleoutMessageBusMessagesReceivedPerSec");
            ScaleoutSendQueueLength = new DCrankPerformanceCounter("ScaleoutSendQueueLength");
            ScaleoutStreamCountBuffering = new DCrankPerformanceCounter("ScaleoutStreamCountBuffering");
            ScaleoutStreamCountOpen = new DCrankPerformanceCounter("ScaleoutStreamCountOpen");
            ScaleoutStreamCountTotal = new DCrankPerformanceCounter("ScaleoutStreamCountTotal");
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
            get;
            private set;

        }

        public IPerformanceCounter ErrorsAllTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsHubInvocationPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsHubInvocationTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsHubResolutionPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsHubResolutionTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsTransportPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ErrorsTransportTotal
        {
            get;
            private set;
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
            get;
            private set;
        }

        public IPerformanceCounter MessageBusBusyWorkers
        {
            get;
            private set;
        }

        public IPerformanceCounter MessageBusMessagesPublishedPerSec
        {
            get;
            private set;

        }

        public IPerformanceCounter MessageBusMessagesPublishedTotal
        {
            get;
            private set;

        }

        public IPerformanceCounter MessageBusMessagesReceivedPerSec
        {
            get;
            private set;

        }

        public IPerformanceCounter MessageBusMessagesReceivedTotal
        {
            get;
            private set;

        }

        public IPerformanceCounter MessageBusSubscribersCurrent
        {
            get;
            private set;
        }

        public IPerformanceCounter MessageBusSubscribersPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter MessageBusSubscribersTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter MessageBusTopicsCurrent
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutErrorsPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutErrorsTotal
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutMessageBusMessagesReceivedPerSec
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutSendQueueLength
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutStreamCountBuffering
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutStreamCountOpen
        {
            get;
            private set;
        }

        public IPerformanceCounter ScaleoutStreamCountTotal
        {
            get;
            private set;
        }
    }
}
