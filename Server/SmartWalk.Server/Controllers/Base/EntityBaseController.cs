using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Orchard;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EntityService;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Shared.Utils;

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
            var result = _entityService.GetEntities(display, EntityType, 0, ViewSettings.ItemsLoad, e => e.Name);

            return View(new ListViewVm<EntityVm>
                {
                    Parameters = new ListViewParametersVm { Display = display },
                    Data = result
                });
        }

        [CompressFilter]
        public ActionResult View(int entityId)
        {
            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            return View(result);
        }

        [CompressFilter]
        public ActionResult Create()
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            return View(new EntityVm { Type = (int)EntityType});
        }

        [CompressFilter]
        public ActionResult Edit(int entityId)
        {
            var result = _entityService.GetEntityById(entityId);
            if (result == null || result.Type != (int)EntityType) return new HttpNotFoundResult();

            var access = _entityService.GetEntityAccess(entityId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            return View(result);
        }

        [CompressFilter]
        public ActionResult Delete(int entityId)
        {
            var access = _entityService.GetEntityAccess(entityId);
            if (access != AccessType.AllowEdit) return new HttpUnauthorizedResult();

            _entityService.DeleteEntity(entityId);

            return RedirectToAction("List");
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEvents(int entityId)
        {
            var result = _eventService.GetEventsByEntity(entityId);
            return Json(result);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult GetEntities(int pageNumber, string query, ListViewParametersVm parameters)
        {
            var result = _entityService.GetEntities(
                parameters.Display,
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
                    onlyMine ? DisplayType.My : DisplayType.All,
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
        public ActionResult Validate(string propName, EntityVm model)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var errors = ValidateEntity(model);
            if (errors.ContainsPropertyError(propName))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ErrorResultVm(errors));
            }

            return Json(true);
        }

        [HttpPost]
        [CompressFilter]
        public ActionResult SaveEntity(EntityVm entityVm)
        {
            if (CurrentSmartWalkUser == null) return new HttpUnauthorizedResult();

            var errors = ValidateEntity(entityVm);
            if (errors.Count > 0)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ErrorResultVm(errors));
            }

            var result = _entityService.SaveEntity(entityVm);
            return Json(result);
        }

        private IList<ValidationError> ValidateEntity(EntityVm model)
        {
            var result = new List<ValidationError>();

            var nameProperty = model.GetPropertyName(p => p.Name);
            if (string.IsNullOrEmpty(model.Name))
            {
                result.Add(new ValidationError(nameProperty, T(EntityTypeName + " name can not be empty.").Text));
            }
            else if (model.Name.Length > 255)
            {
                result.Add(new ValidationError(
                               nameProperty,
                               T(EntityTypeName + " name can not be longer than 255 characters.").Text));
            }
            else if (!_entityService.IsNameUnique(model))
            {
                result.Add(new ValidationError(nameProperty, T(EntityTypeName + " name must be unique.").Text));
            }

            var pictureProperty = model.GetPropertyName(p => p.Picture);
            if (!string.IsNullOrEmpty(model.Picture))
            {
                if (model.Picture.Length > 255)
                {
                    result.Add(new ValidationError(
                                   pictureProperty,
                                   T("Picture url can not be longer than 255 characters.").Text));
                }
                else if (!model.Picture.IsUrlValid())
                {
                    result.Add(new ValidationError(pictureProperty, T("Picture URL has bad format.").Text));
                }
            }

            var descriptionProperty = model.GetPropertyName(p => p.Description);
            if (!string.IsNullOrEmpty(model.Description))
            {
                if (model.Description.Length > 3000)
                {
                    result.Add(new ValidationError(
                                   descriptionProperty,
                                   T("Description can not be longer than 3000 characters.").Text));
                }
            }

            var addressesProperty = model.GetPropertyName(p => p.Addresses);
            var addresses = model.Addresses != null
                ? model.Addresses.Where(v => !v.Destroy).ToArray()
                : new AddressVm[] { };
            for (var i = 0; i < addresses.Length; i++)
            {
                var addressVm = addresses[i];
                result.AddRange(ValidateAddress(
                    addressVm, 
                    string.Format("{0}[{1}].", addressesProperty, i + 1)));
            }

            var contactsProperty = model.GetPropertyName(p => p.Contacts);
            var contacts = model.Contacts != null
                ? model.Contacts.Where(v => !v.Destroy).ToArray()
                : new ContactVm[] { };
            for (var i = 0; i < contacts.Length; i++)
            {
                var contactVm = contacts[i];
                result.AddRange(ValidateContact(
                    contactVm, 
                    string.Format("{0}[{1}].", contactsProperty, i + 1)));
            }

            return result;
        }

        private IEnumerable<ValidationError> ValidateAddress(AddressVm model, string prefix = "")
        {
            var result = new List<ValidationError>();

            
            if (!string.IsNullOrEmpty(model.Address) && model.Address.Length > 255)
            {
                var addressProperty = model.GetPropertyName(p => p.Address);
                result.Add(new ValidationError(
                               prefix + addressProperty,
                               T("Address can not be longer than 255 characters.").Text));
            }

            var latitudeProperty = model.GetPropertyName(p => p.Latitude);
            if (Math.Abs(model.Latitude - 0) < 0.00001)
            {
                result.Add(new ValidationError(
                               prefix + latitudeProperty,
                               T("Latitude can not be empty.").Text));
            }
            else if (model.Latitude < -85 || model.Latitude > 85)
            {
                result.Add(new ValidationError(
                               prefix + latitudeProperty,
                               T("Latitude is out of range.").Text));
            }

            var longitudeProperty = model.GetPropertyName(p => p.Longitude);
            if (Math.Abs(model.Longitude - 0) < 0.00001)
            {
                result.Add(new ValidationError(
                               prefix + longitudeProperty,
                               T("Longitude can not be empty").Text));
            }
            else if (model.Longitude < -180 || model.Longitude > 180)
            {
                result.Add(new ValidationError(
                               prefix + latitudeProperty,
                               T("Longitude is out of range.").Text));
            }

            if (!string.IsNullOrEmpty(model.Tip) && model.Tip.Length > 255)
            {
                var tipProperty = model.GetPropertyName(p => p.Tip);
                result.Add(new ValidationError(
                                prefix + tipProperty,
                                T("Address tip can not be longer than 255 characters.").Text));
            }

            return result;
        }

        private IEnumerable<ValidationError> ValidateContact(ContactVm model, string prefix = "")
        {
            var result = new List<ValidationError>();

            var contactProperty = model.GetPropertyName(p => p.Contact);
            if (string.IsNullOrEmpty(model.Contact))
            {
                result.Add(new ValidationError(
                               prefix + contactProperty,
                               T("Contact can not be empty.").Text));
            }
            else if (model.Contact.Length > 255)
            {
                result.Add(new ValidationError(
                               prefix + contactProperty,
                               T("Contact can not be longer than 255 characters.").Text));
            }

            var titleProperty = model.GetPropertyName(p => p.Title);
            if (!string.IsNullOrEmpty(model.Title))
            {
                if (model.Title.Length > 255)
                {
                    result.Add(new ValidationError(
                                   prefix + titleProperty,
                                   T("Contact title can not be longer than 255 characters.").Text));
                }
            }

            return result;
        }
    }
}