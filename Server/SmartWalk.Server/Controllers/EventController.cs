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
using Orchard.DisplayManagement;
using Orchard.Mvc;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Services.EntityService;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class EventController : BaseController {
        private readonly IOrchardServices _orchardServices;

        private readonly IEventService _eventService;
        private readonly IEntityService _entityService;

        public EventController(IEventService eventService, IEntityService entityService, IOrchardServices orchardServices) {
            _orchardServices = orchardServices;

            _eventService = eventService;
            _entityService = entityService;
        }

        public ActionResult List()
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_eventService.GetUserEvents(user.Record));
        }

        public ActionResult Edit(int eventId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_eventService.GetUserEventVmById(user.Record, eventId));
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

        [HttpPost]
        public ActionResult GetShow(ShowVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            return Json(_entityService.GetShow(item.Id));
        }

        [HttpPost]
        public ActionResult SaveShow(ShowVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            return Json(_entityService.SaveOrAddShow(item).Id);
        }

        [HttpPost]
        public ActionResult DeleteShow(ShowVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            _entityService.DeleteShow(item.Id);

            return Json(true);
        }

        [HttpPost]
        public ActionResult DeleteEventVenue(EntityVm item) {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            _entityService.DeleteEventVenue(item);

            return Json(true);
        }

        [HttpPost]
        public ActionResult AddEventVenue(EntityVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }
            
            return Json(_entityService.AddEventVenue(item));
        }        

        [HttpPost]
        public ActionResult DeleteEvent(EventMetadataVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            _eventService.DeleteEvent(item);

            return Json(true);
        }

        [HttpPost]
        public ActionResult SaveEvent(EventMetadataVm item)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            _eventService.SaveOrAddEvent(item);

            return Json(_eventService.SaveOrAddEvent(item));
        } 
    }
}