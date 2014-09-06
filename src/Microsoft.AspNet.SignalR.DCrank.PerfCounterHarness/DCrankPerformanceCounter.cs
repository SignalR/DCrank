using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class DCrankPerformanceCounter : IPerformanceCounter
    {
        private readonly string _counterName;
        private long _counterValue;

        public DCrankPerformanceCounter(string counterName)
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
            return Interlocked.Decrement(ref _counterValue);
        }

        public long Increment()
        {
            return Interlocked.Increment(ref _counterValue);
        }

        public long IncrementBy(long value)
        {
            return Interlocked.Add(ref _counterValue, value);
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
