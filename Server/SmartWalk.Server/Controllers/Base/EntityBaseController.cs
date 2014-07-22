using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Controllers.Base
{
    public abstract class EntityBaseController : OrchardBaseController
    {
        private readonly IEntityService _entityService;
        private readonly IEventService _eventService;

        protected EntityBaseController(
            IOrchardServices orchardServices,
            IEntityService entityService,
            IEventService eventService)
            : base(orchardServices)
        {
            _entityService = entityService;
            _eventService = eventService;
        }

        protected abstract EntityType EntityType { get; }

        private string EntityTypeName
        {
            get { return EntityType == EntityType.Venue ? "Venue" : "Organizer"; }
        }

        [CompressFilter]
        public ActionResult List(DisplayType display)
        {
            var result = _entityService.GetEntities(
                CurrentSmartWalkUser == null || display == DisplayType.All
                    ? null
                    : CurrentSmartWalkUser.Record,
                EntityType,
                0,
                ViewSettings.ItemsLoad,
                e => e.Name);

            return View(new ListViewVm<EntityVm>
                {
                    Parameters = new ListViewParametersVm { Display = display },
                    Data = result
                });
        }

        [CompressFilter]
        public ActionResult View(int entityId)
        {
            var entityVm = _entityService.GetEntityVmById(entityId, EntityType);
            if (entityVm.Id != entityId) return new HttpNotFoundResult();

            return View(entityVm);
        }

        [CompressFilter]
        public ActionResult Create()
        {
            return Edit(0);
        }

        [CompressFilter]
        public ActionResult Edit(int entityId)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var entityVm = _entityService.GetEntityVmById(entityId, EntityType);
            if (entityVm.Id != entityId) return new HttpNotFoundResult();

            var access = _entityService.GetEntityAccess(CurrentSmartWalkUser.Record, entityId);
            if (access == AccessType.AllowEdit) return View(entityVm);

            return new HttpUnauthorizedResult();
        }

        [CompressFilter]
        public ActionResult Delete(int entityId)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            _entityService.DeleteEntity(entityId);

            return RedirectToAction("List");
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEvents(int entityId)
        {
            var result = _eventService.GetEntityEvents(entityId);
            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEntities(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var result = _entityService.GetEntities(
                CurrentSmartWalkUser == null || parameters.Display == DisplayType.All
                    ? null
                    : CurrentSmartWalkUser.Record,
                EntityType,
                pageNumber,
                ViewSettings.ItemsLoad,
                e => e.Name,
                false,
                query);

            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult AutoCompleteEntity(string term, bool onlyMine = true, int[] excludeIds = null)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            return Json(
                _entityService.GetEntities(
                    onlyMine ? CurrentSmartWalkUser.Record : null,
                    EntityType,
                    0,
                    ViewSettings.ItemsLoad,
                    e => e.Name,
                    false,
                    term,
                    excludeIds));
        }

        [HttpPost]
        [CompressFilter]
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
        [CompressFilter]
        public ActionResult SaveEntity(EntityVm host)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            try
            {
                var errors = ValidateModel(host);
                return Json(errors.Count > 0 ? null : _entityService.SaveOrAddEntity(CurrentSmartWalkUser.Record, host));
            }
            catch
            {
                return Json(false);
            }
        }

        private IDictionary<string, string> ValidateModel(EntityVm model)
        {
            var result = new Dictionary<string, string>();

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

            return result;
        }

        // TODO: To validate address on event Save
        private IDictionary<string, string> ValidateAddress(AddressVm model)
        {
            var res = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(model.Address))
                res.Add("Address", T("Address can not be empty!").Text);
            else if (model.Address.Length > 255)
                res.Add("Address", T("Address can not be larger than 255 characters!").Text);

            if (!string.IsNullOrEmpty(model.Tip))
            {
                if (model.Tip.Length > 255)
                    res.Add("Tip", T("Address tip can not be larger than 255 characters!").Text);
            }

            return res;
        }

        // TODO: To validate contact on event Save
        private IDictionary<string, string> ValidateContact(ContactVm model)
        {
            var res = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(model.Contact))
                res.Add("Contact", T("Contact can not be empty!").Text);
            else if (model.Contact.Length > 255)
                res.Add("Contact", T("Contact can not be larger than 255 characters!").Text);

            if (!string.IsNullOrEmpty(model.Title))
            {
                if (model.Title.Length > 255)
                    res.Add("Title", T("Contact title can not be larger than 255 characters!").Text);
            }


            return res;
        }
    }
}