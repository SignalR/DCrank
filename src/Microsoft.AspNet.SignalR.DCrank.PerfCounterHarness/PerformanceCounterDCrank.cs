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

        // Only for per/sec counters
        private long _latestCounterSample;
        private long _counterRate;

        private DCrankPerformanceCounterType _counterType;

        public DCrankPerformanceCounter(string counterName, DCrankPerformanceCounterType perfCounterType)
        {
            _counterName = counterName;
            _counterType = perfCounterType;

            if (_counterType == DCrankPerformanceCounterType.PerSecRate)
            {
                UpdateCounterRate();
            }
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
            Interlocked.Exchange(ref _counterValue, --_counterValue);
            return _counterValue;
        }

        public long Increment()
        {
            Interlocked.Exchange(ref _counterValue, ++_counterValue);
            return _counterValue;
        }

        public long IncrementBy(long value)
        {
            Interlocked.Exchange(ref _counterValue, _counterValue + value);
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
                if (_counterType == DCrankPerformanceCounterType.Total)
                {
                    return _counterValue;
                }
                else
                {
                    return _counterRate;
                }
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

        private void UpdateCounterRate()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Interlocked.Exchange(ref _counterRate, _counterValue - _latestCounterSample);
                    Interlocked.Exchange(ref _latestCounterSample, _counterValue);

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });
        }
    }
}
