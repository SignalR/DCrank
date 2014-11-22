using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.DCrank.TestController.Models;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Controllers
{
    public class RunController : Controller
    {
        // POST: LoadDefinition
        public ActionResult LoadDefinition(string runType)
        {
            var runDefinition = new RunDefinition
            {
                Type = "manual",
                RunParameters = new List<RunParameter>
                {
                    new RunParameter
                    {
                        Label = "Target URL",
                        Type = "url",
                        Placeholder = "URL",
                        Value = "http://localhost:24037/"
                    },
                    new RunParameter
                    {
                        Label = "Agents",
                        Type = "number",
                        Placeholder = "Number of agents",
                        Value = 1
                    },
                    new RunParameter
                    {
                        Label = "Connections",
                        Type = "number",
                        Placeholder = "Number of connections",
                        Value = 3
                    },
                    new RunParameter
                    {
                        Label = "Message Size",
                        Type = "number",
                        Placeholder = "Bytes per message",
                        Value = 32
                    }
                }
            };

            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(runDefinition),
                ContentEncoding = Encoding.UTF8
            };
        }

        // POST: Start
        [HttpPost]
        public ActionResult Start(RunDefinition runDefinition)
        {
            var x = runDefinition;

            return null;
        }
    }
}