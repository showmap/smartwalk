using System;
using System.Linq;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EntityService
{
    public static class ViewModelFactory
    {
        public static EntityVm CreateViewModel(EntityRecord record, LoadMode mode)
        {
            if (record == null) throw new ArgumentNullException("record");

            var result = new EntityVm { Id = record.Id };

            if (mode == LoadMode.Compact || mode == LoadMode.Full)
            {
                result.Name = record.Name;
                result.Abbreviation = record.Name.GetAbbreviation(2);
                result.Picture = record.Picture;
                result.Addresses = record.AddressRecords
                    .Where(ar => !ar.IsDeleted)
                    .Select(CreateViewModel)
                    .ToArray();
            }

            if (mode == LoadMode.Full)
            {
                result.Type = (EntityType)record.Type;
                result.Description = record.Description;
                result.Contacts = record.ContactRecords
                    .Where(c => !c.IsDeleted)
                    .Select(CreateViewModel)
                    .ToArray();
            }

            return result;
        }

        public static EntityVm CreateViewModel(EntityRecord record, EventEntityDetailRecord detailRecord)
        {
            var result = CreateViewModel(record, LoadMode.Full);

            if (detailRecord != null)
            {
                result.EventDetail = CreateViewModel(detailRecord);
            }

            return result;
        }

        public static void UpdateByViewModel(EntityRecord record, EntityVm entityVm)
        {
            record.DateModified = DateTime.UtcNow;

            record.Type = (byte)entityVm.Type;
            record.Name = entityVm.Name.TrimIt().StripTags();
            record.Picture = entityVm.Picture.TrimIt().StripTags();
            record.Description = entityVm.Description.TrimIt().StripTags();
        }

        public static void UpdateByViewModel(AddressRecord record, AddressVm addressVm)
        {
            record.Address = addressVm.Address.TrimIt().StripTags();
            record.Latitude = addressVm.Latitude;
            record.Longitude = addressVm.Longitude;
            record.Tip = addressVm.Tip.TrimIt().StripTags();

            record.IsDeleted = addressVm.Destroy;
        }

        public static void UpdateByViewModel(ContactRecord record, ContactVm contactVm)
        {
            record.Type = (byte)contactVm.Type;
            record.Title = contactVm.Title.TrimIt().StripTags();
            record.Contact = contactVm.Contact.TrimIt().StripTags();

            record.IsDeleted = contactVm.Destroy;
        }

        public static void UpdateByViewModel(EventEntityDetailRecord record, EventEntityDetailVm detailVm)
        {
            record.SortOrder = detailVm != null ? detailVm.SortOrder : null;
            record.Description = detailVm != null ? detailVm.Description.TrimIt().StripTags() : null;
        }

        private static AddressVm CreateViewModel(AddressRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");

            return new AddressVm
                {
                    Id = record.Id,
                    Address = record.Address,
                    Latitude = record.Latitude,
                    Longitude = record.Longitude,
                    Tip = record.Tip,
                };
        }

        private static ContactVm CreateViewModel(ContactRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");

            return new ContactVm
                {
                    Id = record.Id,
                    Type = (ContactType)record.Type,
                    Title = record.Title,
                    Contact = record.Contact
                };
        }

        private static EventEntityDetailVm CreateViewModel(EventEntityDetailRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");

            return new EventEntityDetailVm
                {
                    SortOrder = record.SortOrder,
                    Description = record.Description
                };
        }
    }
}