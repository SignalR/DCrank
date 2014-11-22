using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Models
{
    public class RunParameter
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Placeholder { get; set; }
        public dynamic Value { get; set; }
    }
}