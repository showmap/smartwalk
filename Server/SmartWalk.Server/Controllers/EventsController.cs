using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;
using Orchard.Themes;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class EventsController : BaseController {
        private readonly IOrchardServices _orchardServices;

        private readonly IEventService _eventService;

        public EventsController(IEventService eventService, IOrchardServices orchardServices) {
            _orchardServices = orchardServices;

            _eventService = eventService;
        }

        public ActionResult Index()
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_eventService.GetUserEvents(user.Record));
        }

        [HttpPost]
        public ActionResult GetEvents() {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return Json(_eventService.GetUserEvents(user.Record));
        }
    }
}