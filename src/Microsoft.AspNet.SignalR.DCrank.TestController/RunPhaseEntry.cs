using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.AspNet.SignalR.DCrank.TestController
{
    public class RunPhaseEntry
    {
        public int RunPhaseEntryId { get; set; }
        public int RunId { get; set; }
        public int PhaseId { get; set; }
        public DateTimeOffset StartTimestamp { get; set; }
        public DateTimeOffset StopTimestamp { get; set; }
    }
}