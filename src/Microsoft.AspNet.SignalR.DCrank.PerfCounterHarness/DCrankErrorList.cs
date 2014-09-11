using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ErrorList.Add(new Tuple<DateTimeOffset, string>(DateTime.UtcNow, ex.ToString()));
        }
    }
}
