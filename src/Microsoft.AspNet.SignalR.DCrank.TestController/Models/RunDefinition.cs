using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Models
{
    public class RunDefinition
    {
        public string Type { get; set; }
        public List<RunParameter> RunParameters { get; set; }
    }
}