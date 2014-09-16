using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class DCrankPerformanceCounterManager : IPerformanceCounterManager
    {
        public DCrankPerformanceCounterManager()
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

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionMessagesReceived", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ConnectionMessagesReceivedPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionMessagesReceived", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ConnectionMessagesReceivedTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionMessagesSent", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ConnectionMessagesSentPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionMessagesSent", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ConnectionMessagesSentTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionsConnected", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ConnectionsConnected
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionsCurrent", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ConnectionsCurrent
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionsDisconnected", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ConnectionsDisconnected
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ConnectionsReconnected", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ConnectionsReconnected
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsAll", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ErrorsAllPerSec
        {
            get;
            private set;

        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsAll", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ErrorsAllTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsHubInvocation", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ErrorsHubInvocationPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsHubInvocation", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ErrorsHubInvocationTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsHubResolution", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ErrorsHubResolutionPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsHubResolution", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ErrorsHubResolutionTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsTransport", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ErrorsTransportPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ErrorsTransport", CounterType = PerformanceCounterType.Total)]
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

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusAllocatedWorkers", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusAllocatedWorkers
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusBusyWorkers", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusBusyWorkers
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusMessagesPublished", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter MessageBusMessagesPublishedPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusMessagesPublished", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusMessagesPublishedTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusMessagesReceived", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter MessageBusMessagesReceivedPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusMessagesReceived", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusMessagesReceivedTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusSubscribersCurrent", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusSubscribersCurrent
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusSubscribers", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter MessageBusSubscribersPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusSubscribers", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusSubscribersTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "MessageBusTopics", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter MessageBusTopicsCurrent
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutErrors", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ScaleoutErrorsPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutErrors", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ScaleoutErrorsTotal
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutMessageBusMessages", CounterType = PerformanceCounterType.PerSecRate)]
        public IPerformanceCounter ScaleoutMessageBusMessagesReceivedPerSec
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutSendQueueLength", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ScaleoutSendQueueLength
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutStreamCountBuffering", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ScaleoutStreamCountBuffering
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutStreamCountOpen", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ScaleoutStreamCountOpen
        {
            get;
            private set;
        }

        [PerformanceCounterAttribute(BaseCounterName = "ScaleoutStreamCount", CounterType = PerformanceCounterType.Total)]
        public IPerformanceCounter ScaleoutStreamCountTotal
        {
            get;
            private set;
        }
    }
}
