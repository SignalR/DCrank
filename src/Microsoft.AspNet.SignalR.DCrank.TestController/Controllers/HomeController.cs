using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.AspNet.SignalR.DCrank.TestController.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Kill()
        {
            Process.GetCurrentProcess().Kill();

            return Json("Killed");
        }
    }
}