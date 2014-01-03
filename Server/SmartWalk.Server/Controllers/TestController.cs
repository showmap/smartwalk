using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Themes;

namespace Store.GsmCounters.Controllers
{
    [HandleError, Themed]
    public class TestController : Controller
    {
        public ActionResult TestFb() {
            return View();
        }

        public ActionResult TestFbE()
        {
            return View();
        }

        public ActionResult Places() {
            return View();
        }
    }
}