using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.DCrank.TestController.Models;
using Newtonsoft.Json;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Controllers
{
    public class RunController : Controller
    {
        // POST: LoadDefinitions
        public ActionResult LoadDefinitions(string runType)
        {
            // ToDo: Generate definitions from POCO with attributes?
            var runDefinitions = new List<RunDefinition>
            {
                new RunDefinition
                {
                    Type = "Auto",
                    RunParameters = new List<RunParameter>
                    {
                        new RunParameter
                        {
                            Label = "Target URL",
                            Type = "url",
                            Placeholder = "URL",
                            Value = "http://localhost:24037/TestConnection"
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
                            Label = "Workers per agent",
                            Type = "number",
                            Placeholder = "Number of workers",
                            Value = 3
                        },
                        new RunParameter
                        {
                            Label = "Message Size",
                            Type = "number",
                            Placeholder = "Bytes per message",
                            Value = 32
                        },
                        new RunParameter
                        {
                            Label = "Send Interval",
                            Type = "number",
                            Placeholder = "Seconds between sends",
                            Value = 1
                        },
                        new RunParameter
                        {
                            Label = "Send Duration",
                            Type = "number",
                            Placeholder = "Send phase duration in seconds",
                            Value = 300
                        }
                    }
                }
            };

            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(runDefinitions),
                ContentEncoding = Encoding.UTF8
            };
        }

        // POST: LoadStatus
        public ActionResult LoadStatus()
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(new
                {
                    State = StateModel.Instance.GetRunState().ToString(),
                    ActiveRun = StateModel.Instance.GetRunDefinition()
                }),
                ContentEncoding = Encoding.UTF8
            };
        }


        // POST: Start
        [HttpPost]
        public ActionResult Start(RunDefinition runDefinition)
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(new
                {
                    Started = StateModel.Instance.StartRun(runDefinition)
                }),
                ContentEncoding = Encoding.UTF8
            };
        }

        [HttpPost]
        public ActionResult Terminate()
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(new
                {
                    Terminated = StateModel.Instance.TerminateRun()
                }),
                ContentEncoding = Encoding.UTF8
            };
        }
    }
}