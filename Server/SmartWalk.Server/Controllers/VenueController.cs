using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Themes;
using Orchard.ContentManagement;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Services.EventService;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class VenueController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IEntityService _entityService;
        private readonly IEventService _eventService;

        public VenueController(IOrchardServices orchardServices, IEntityService entityService, IEventService eventService)
        {
            _orchardServices = orchardServices;

            _entityService = entityService;
            _eventService = eventService;
        }

        public ActionResult List() {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_entityService.GetUserEntities(user.Record, EntityType.Venue));
        }

        public ActionResult View(int entityId) {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            return View(_entityService.GetEntityVmById(entityId, EntityType.Venue));
        }

        public ActionResult Edit(int entityId) {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }            

            return View(_entityService.GetEntityVmById(entityId, EntityType.Venue));
        }

        public ActionResult Delete(int entityId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            _entityService.DeleteEntity(entityId);

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult GetEvents(int entityId) {
            var res = _eventService.GetEntityEvents(entityId);
            return Json(res);
        }

        [HttpPost]
        public ActionResult SaveOrAdd(EntityVm venue)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            try
            {
                return Json(_entityService.SaveOrAddEntity(user.Record, venue));
            }
            catch
            {
                return Json(false);
            }
        }
    }
}