using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.DCrank.Crank
{
    public class CrankListener : TextWriterTraceListener
    {
        public CrankListener()
            : base()
        {
            Writer = new StreamWriter("dcrank_log.txt");
        }
    }
}
