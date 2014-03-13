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

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class HostController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IEntityService _entityService;

        public HostController(IOrchardServices orchardServices, IEntityService entityService) {
            _orchardServices = orchardServices;

            _entityService = entityService;
        }

        public ActionResult List() {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(_entityService.GetUserEntities(user.Record, EntityType.Host));
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
        public ActionResult SaveOrAdd(EntityVm host)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            try
            {
                _entityService.SaveOrAddEntity(user.Record, host);
            }
            catch
            {
                return Json(false);
            }

            return Json(true);
        }
    }
}