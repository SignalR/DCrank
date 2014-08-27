using System;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterDCrank : IPerformanceCounter
    {
        private long _counterValue;
        private string _counterName;

        public PerformanceCounterDCrank(string counterName)
        {
            _counterName = counterName;
        }
        
        public void Close()
        {
            throw new NotImplementedException();
        }

        public string CounterName
        {
            get { return _counterName; }
        }

        public long Decrement()
        {
            return --_counterValue;
        }

        public long Increment()
        {
            return ++_counterValue;
        }

        public long IncrementBy(long value)
        {
            _counterValue += value;
            return _counterValue;
        }

        public System.Diagnostics.CounterSample NextSample()
        {
            throw new NotImplementedException();
        }

        public long RawValue
        {
            get
            {
                return _counterValue;
            }
            set
            {
                _counterValue = value;
            }
        }

        public void RemoveInstance()
        {
            throw new NotImplementedException();
        }
    }
}
