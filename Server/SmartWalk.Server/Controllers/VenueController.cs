using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Themes;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Server.Extensions;
using System.Linq;

namespace SmartWalk.Server.Controllers
{
    [HandleError, Themed]
    public class VenueController : BaseController
    {
        private readonly IOrchardServices _orchardServices;

        private readonly IEntityService _entityService;
        private readonly IEventService _eventService;

        public Localizer T { get; set; }

        public VenueController(IOrchardServices orchardServices, IEntityService entityService, IEventService eventService)
        {
            _orchardServices = orchardServices;

            _entityService = entityService;
            _eventService = eventService;

            T = NullLocalizer.Instance;
        }

        public ActionResult List(ListViewParametersVm parameters) {
            parameters.IsLoggedIn = _orchardServices.WorkContext.CurrentUser != null;

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(new ListViewVm {Parameters = parameters, Data = _entityService.GetEntities(user == null ? null : user.Record, EntityType.Venue, 0, ViewSettings.ItemsLoad, e => e.Name, false, "")});
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

        private IDictionary<string, string> ValidateModel(EntityVm model) {
            var res = new Dictionary<string, string>();

            #region Name
            if (string.IsNullOrEmpty(model.Name))
                res.Add("Name", T("Venue name can not be empty!").Text);
            else if(model.Name.Length > 255)
                res.Add("Name", T("Venue name can not be larger than 255 characters!").Text);
            else if (_entityService.IsNameExists(model, EntityType.Venue))
                res.Add("Name", T("Venue name must be unique!").Text);
            #endregion

            #region Picture
            if (!string.IsNullOrEmpty(model.Picture)) {
                if(model.Picture.Length > 255)
                    res.Add("Picture", T("Picture url can not be larger than 255 characters!").Text);
                else if (!model.Picture.IsUrlValid())
                    res.Add("Picture", T("Picture url is in bad format!").Text);
            }
            #endregion

            return res;
        }

        [HttpPost]
        public ActionResult ValidateModel(string propName, EntityVm model) {
            var errors = ValidateModel(model);

            if (errors.ContainsKey(propName)) {
                HttpContext.Response.StatusCode = 400;
                return Json(new {Message = errors[propName]});
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult AutoCompleteVenue(string term, int eventId, EventMetadataVm currentEvent)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
            var res = _entityService.GetAccesibleUserVenues(user.Record, eventId, 0, ViewSettings.ItemsLoad, term);

            if (eventId == 0 && currentEvent != null)
                res = res.Where(e => currentEvent.AllVenues.Where(v => v.State != VmItemState.Deleted && v.Id > 0).All(v => v.Id != e.Id)).ToList();

            return Json(res);
        }

        [HttpPost]
        public ActionResult GetVenues(int pageNumber, string query, ListViewParametersVm parameters) {
            SmartWalkUserPart user = null;

            if (parameters.IsLoggedIn) {
                if (_orchardServices.WorkContext.CurrentUser == null)
                {
                    return new HttpUnauthorizedResult();
                }

                user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();                
            }

            return Json(_entityService.GetEntities(user == null ? null : user.Record, EntityType.Venue, pageNumber, ViewSettings.ItemsLoad, e => e.Name, false, query));
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

            try {
                var errors = ValidateModel(venue);
                return Json(errors.Count > 0 ? null : _entityService.SaveOrAddEntity(user.Record, venue));
            }
            catch
            {
                return Json(false);
            }
        }
    }
}