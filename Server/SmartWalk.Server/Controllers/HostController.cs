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
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class HostController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IEntityService _entityService;
        private readonly IEventService _eventService;

        public HostController(IOrchardServices orchardServices, IEntityService entityService, IEventService eventService) {
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

            return View(_entityService.GetUserEntities(user.Record, EntityType.Host, 0, SmartWalkSettings.InitialItemsLoad, e => e.Name, false));
        }

        public ActionResult View(int entityId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            return View(_entityService.GetEntityVmById(entityId, EntityType.Host));
        }

        public ActionResult Edit(int entityId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            return View(_entityService.GetEntityVmById(entityId, EntityType.Host));
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
        public ActionResult GetHosts(int pageNumber)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return Json(_entityService.GetUserEntities(user.Record, EntityType.Host, pageNumber, SmartWalkSettings.ItemsLoad, e => e.Name, false));
        }

        [HttpPost]
        public ActionResult GetEvents(int entityId)
        {
            var res = _eventService.GetEntityEvents(entityId);
            return Json(res);
        }

        [HttpPost]
        public ActionResult SaveOrAdd(EntityVm host)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            try
            {
                return Json(_entityService.SaveOrAddEntity(user.Record, host));
            }
            catch
            {
                return Json(false);
            }
        }
    }
}