using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Models;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers.Base
{
    public abstract class EntityBaseController : BaseController
    {
        private readonly IEntityService _entityService;
        private readonly IEventService _eventService;
        private readonly IOrchardServices _orchardServices;

        protected EntityBaseController(
            IOrchardServices orchardServices,
            IEntityService entityService,
            IEventService eventService)
        {
            _orchardServices = orchardServices;
            _entityService = entityService;
            _eventService = eventService;
        }

        protected abstract EntityType EntityType { get; }

        private string EntityTypeName
        {
            get { return EntityType == EntityType.Venue ? "Venue" : "Organizer"; }
        }

        public ActionResult List(ListViewParametersVm parameters)
        {
            parameters.IsLoggedIn = _orchardServices.WorkContext.CurrentUser != null;

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return View(
                new ListViewVm
                {
                    Parameters = parameters,
                    Data =
                        _entityService.GetEntities(
                            user == null ? null : user.Record,
                            EntityType,
                            0,
                            ViewSettings.ItemsLoad,
                            e => e.Name,
                            false,
                            "")
                });
        }

        public ActionResult View(int entityId)
        {
            var entityVm = _entityService.GetEntityVmById(entityId, EntityType);
            if (entityVm.Id != entityId) return new HttpNotFoundResult();

            return
                View(
                    new ViewParametersVm
                    {
                        IsReadOnly = _orchardServices.WorkContext.CurrentUser == null,
                        Data = entityVm
                    });
        }

        public ActionResult Create()
        {
            return Edit(0);
        }

        public ActionResult Edit(int entityId)
        {
            var entityVm = _entityService.GetEntityVmById(entityId, EntityType);
            if (entityVm.Id != entityId) return new HttpNotFoundResult();

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
            if (user == null) return new HttpUnauthorizedResult();

            var access = _entityService.GetEntityAccess(user.Record, entityId);
            if (access == AccessType.AllowEdit) return View(entityVm);

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
        public ActionResult GetEvents(int entityId)
        {
            var result = _eventService.GetEntityEvents(entityId);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetEntities(int pageNumber, string query, ListViewParametersVm parameters)
        {
            SmartWalkUserPart user = null;

            if (parameters.IsLoggedIn)
            {
                if (_orchardServices.WorkContext.CurrentUser == null)
                {
                    return new HttpUnauthorizedResult();
                }

                user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
            }

            return Json(
                _entityService.GetEntities(
                    user == null ? null : user.Record,
                    EntityType, 
                    pageNumber,
                    ViewSettings.ItemsLoad, 
                    e => e.Name, 
                    false, 
                    query));
        }

        [HttpPost]
        public ActionResult AutoCompleteEntity(string term, bool onlyMine = true, int[] excludeIds = null)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            return Json(
                _entityService.GetEntities(
                    onlyMine ? user.Record : null,
                    EntityType,
                    0,
                    ViewSettings.ItemsLoad,
                    e => e.Name,
                    false,
                    term,
                    excludeIds));
        }

        [HttpPost]
        public ActionResult ValidateModel(string propName, EntityVm model)
        {
            var errors = ValidateModel(model);

            if (errors.ContainsKey(propName))
            {
                HttpContext.Response.StatusCode = 400;
                return Json(new { Message = errors[propName] });
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult SaveEntity(EntityVm host)
        {
            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpUnauthorizedResult();
            }

            var user = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            try
            {
                var errors = ValidateModel(host);
                return Json(errors.Count > 0 ? null : _entityService.SaveOrAddEntity(user.Record, host));
            }
            catch
            {
                return Json(false);
            }
        }

        private IDictionary<string, string> ValidateModel(EntityVm model)
        {
            var result = new Dictionary<string, string>();

            #region Name

            if (string.IsNullOrEmpty(model.Name))
            {
                result.Add("Name", T(EntityTypeName + " name can not be empty!").Text);
            }
            else if (model.Name.Length > 255)
            {
                result.Add("Name", T(EntityTypeName + " name can not be larger than 255 characters!").Text);
            }
            else if (_entityService.IsNameExists(model, EntityType))
            {
                result.Add("Name", T(EntityTypeName + " name must be unique!").Text);
            }

            #endregion

            #region Picture

            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                {
                    result.Add("Picture", T("Picture url can not be larger than 255 characters!").Text);
                }
                else if (!model.Picture.IsUrlValid())
                {
                    result.Add("Picture", T("Picture url is in bad format!").Text);
                }
            }

            #endregion

            return result;
        }
    }
}