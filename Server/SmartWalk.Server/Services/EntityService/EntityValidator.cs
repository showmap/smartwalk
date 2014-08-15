using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Orchard.Localization;
using SmartWalk.Server.Extensions;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EntityService
{
    public class EntityValidator
    {
        private readonly IEntityService _entityService;
        private readonly EntityType _entityType;
        private readonly Localizer _localizer;

        public EntityValidator(
            IEntityService entityService,
            EntityType entityType,
            Localizer localizer)
        {
            _entityType = entityType;
            _entityService = entityService;
            _localizer = localizer;
        }

        private Localizer T { get { return _localizer; } }

        private string EntityTypeName
        {
            get { return _entityType == EntityType.Venue ? "Venue" : "Organizer"; }
        }

        public IList<ValidationError> ValidateEntity(EntityVm model)
        {
            var access = _entityService.GetEntitiesAccess();
            if (access != AccessType.AllowEdit) 
                throw new SecurityException("Validation is prohibited due to security restrictions");

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