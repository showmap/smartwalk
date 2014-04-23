using System.Globalization;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Themes;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

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

        public ActionResult List(ListViewParametersVm parameters) {
            parameters.IsLoggedIn = _orchardServices.WorkContext.CurrentUser != null;

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(new ListViewVm {Parameters = parameters, Data = _entityService.GetEntities(user == null ? null : user.Record, EntityType.Venue, 0, ViewSettings.ItemsLoad, null, e => e.Name, false)});
        }

        public ActionResult View(int entityId) {
            var item = _entityService.GetEntityVmById(entityId, EntityType.Venue);

            if (item.Id != entityId)
                return new HttpNotFoundResult();

            return View(new ViewParametersVm { IsReadOnly = _orchardServices.WorkContext.CurrentUser == null, Data = item });
        }

        public ActionResult Edit(int entityId) {
            var item = _entityService.GetEntityVmById(entityId, EntityType.Venue);

            if (item.Id != entityId)
                return new HttpNotFoundResult();

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            if(user == null)
                return new HttpUnauthorizedResult();

            var access = _entityService.GetEntityAccess(user.Record, entityId);

            if(access == AccessType.AllowEdit)
                return View(item);

            return new HttpUnauthorizedResult();
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
        public ActionResult AutoCompleteVenue(string term, int eventId)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return Json(_entityService.GetAccesibleUserVenues(user.Record, eventId, 0, ViewSettings.ItemsLoad, e => (string.IsNullOrEmpty(term) || e.Name.ToLower(CultureInfo.InvariantCulture).Contains(term.ToLower(CultureInfo.InvariantCulture)))));
        }

        [HttpPost]
        public ActionResult GetVenues(int pageNumber, ListViewParametersVm parameters) {
            SmartWalkUserPart user = null;

            if (parameters.IsLoggedIn) {
                if (_orchardServices.WorkContext.CurrentUser == null)
                {
                    return new HttpUnauthorizedResult();
                }

                user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();                
            }

            return Json(_entityService.GetEntities(user == null ? null : user.Record, EntityType.Venue, pageNumber, ViewSettings.ItemsLoad, null, e => e.Name, false));
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