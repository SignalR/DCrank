using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness
{
    public static class DCrankErrorList
    {
        public static List<Tuple<DateTimeOffset, string>> ErrorList { get; private set; }

        static DCrankErrorList()
        {
            ErrorList = new List<Tuple<DateTimeOffset, string>>();
        }

        public static void AddError(Exception ex)
        {
            ErrorList.Add(new Tuple<DateTimeOffset, string>(DateTimeOffset.UtcNow, ex.ToString()));
        }
    }
}
