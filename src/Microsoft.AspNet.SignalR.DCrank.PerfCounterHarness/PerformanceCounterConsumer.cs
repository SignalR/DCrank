using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public class PerformanceCounterConsumer
    {
        private readonly IPerformanceCounterManager _perfCounterManager;
        private readonly string _connectionString;
        private readonly TimeSpan _updateInterval;

        public PerformanceCounterConsumer(IPerformanceCounterManager perfCounterManager, string connectionString, TimeSpan updateInterval)
        {
            _perfCounterManager = perfCounterManager;
            _connectionString = connectionString;
            _updateInterval = updateInterval;
        }

        public void StartWritingPerformanceCounters()
        {
            Task.Run(() =>
            {
                var properties = _perfCounterManager.GetType().GetProperties();

                var definitionList = new List<PerformanceCounterDefinition>();

                foreach (var property in properties)
                {
                    var propertyAttributes = (PerformanceCounterAttribute[])property.GetCustomAttributes(typeof(PerformanceCounterAttribute), false);

                    // Constructing definition
                    if (propertyAttributes.Length > 0)
                    {
                        definitionList.Add(new PerformanceCounterDefinition()
                        {
                            Name = property.Name,
                            Type = propertyAttributes[0].CounterType,
                            ValueId = propertyAttributes[0].BaseCounterName
                        });
                    }
                }

                while (true)
                {
                    using (var database = new PerformanceCounterSampleContext(_connectionString))
                    {
                        try
                        {
                            var valueList = new List<PerformanceCounterValues>();

                            foreach (var property in properties)
                            {
                                var propertyAttributes = (PerformanceCounterAttribute[])property.GetCustomAttributes(typeof(PerformanceCounterAttribute), false);

                                if (propertyAttributes.Length > 0)
                                {
                                    // Constructing value
                                    if (propertyAttributes[0].CounterType == PerformanceCounterType.Total)
                                    {
                                        valueList.Add(new PerformanceCounterValues()
                                        {
                                            ValueId = (propertyAttributes[0]).BaseCounterName,
                                            Value = ((IPerformanceCounter)property.GetValue(_perfCounterManager)).RawValue
                                        });
                                    }
                                }
                            }

                            var perfCounterJsonValue = JsonConvert.SerializeObject(new PerformanceCounterJsonDefinition
                            {
                                Definitions = definitionList,
                                Values = valueList
                            });

                            var perfCounterSample = new PerformanceCounterSample
                            {
                                Timestamp = DateTime.UtcNow,
                                PerformanceCounterJsonBlob = perfCounterJsonValue
                            };

                            database.PerformanceCounterSamples.Add(perfCounterSample);
                            database.SaveChanges();
                        }
                        catch (DbUpdateException ex)
                        {
                            DCrankErrorList.AddError(ex);
                        }
                    }

                    Thread.Sleep(_updateInterval);
                }
            });

        }
    }
}
