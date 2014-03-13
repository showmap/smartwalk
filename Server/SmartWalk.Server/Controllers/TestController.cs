using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Data;
using Orchard.Themes;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;

namespace Store.GsmCounters.Controllers
{
    [HandleError, Themed]
    public class TestController : Controller {
        private readonly IRepository<EntityRecord> _entityRepository;


        public TestController(IRepository<EntityRecord> entityRepository) {
            _entityRepository = entityRepository;
        }

        public ActionResult TestFb() {

            var res = _entityRepository.Table.Select(e => e.EventMetadataRecords
                                         .OrderByDescending(em => em.StartTime)
                                         .FirstOrDefault(em => em.RegionRecord.Region.Contains("San Francisco"))).ToArray();

            return View();
        }

        public ActionResult TestFbE()
        {
            return View();
        }

        public ActionResult Places() {
            return View();
        }

        public ActionResult FSquare() {
            return View();
        }

        public ActionResult Index() {
            return View();
        }
    }
}