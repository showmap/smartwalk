using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EntityService
{
    public static class ViewModelContractFactory
    {
        public static EntityVm CreateViewModelContract(EntityRecord record)
        {
            if (record == null)
                return null;

            return new EntityVm
            {
                Id = record.Id,
                State = VmItemState.Normal,
                UserId = record.SmartWalkUserRecord.Id,
                Type = record.Type,
                Name = record.Name,
                Picture = record.Picture,
                Description = record.Description,
                AllContacts = record.ContactRecords.Select(CreateViewModelContract).ToList(),
                AllAddresses =  record.AddressRecords.Select(CreateViewModelContract).ToList(),
            };
        }

        public static EntityVm CreateViewModelContract(EntityRecord record, EventMetadataRecord metadata)
        {
            if (record == null)
                return null;

            var res = CreateViewModelContract(record);

            if (metadata.ShowRecords.All(s => s.EntityRecord.Id != record.Id))
                res.State = VmItemState.Hidden;

            res.EventMetadataId = metadata.Id;
            res.AllShows = metadata.ShowRecords.Where(s => s.EntityRecord.Id == record.Id && !s.IsDeleted).Select(CreateViewModelContract).ToList();

            return res;
        }

        public static ShowVm CreateViewModelContract(ShowRecord record)
        {
            if (record == null)
                return null;

            return new ShowVm
            {
                Id = record.Id,
                EventMetadataId = record.EventMetadataRecord.Id,
                State = record.IsReference ? VmItemState.Hidden : VmItemState.Normal,
                VenueId = record.EntityRecord.Id,
                Title = record.Title,
                Description = record.Description,
                StartDate = record.StartTime.HasValue ? record.StartTime.Value.ToString("d", CultureInfo.InvariantCulture) : "",
                StartTime = record.StartTime.HasValue ? record.StartTime.Value.ToString("t", CultureInfo.InvariantCulture) : "",
                EndDate = record.EndTime.HasValue ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture) : "",
                EndTime = record.EndTime.HasValue ? record.EndTime.Value.ToString("t", CultureInfo.InvariantCulture) : "",
                IsReference = record.IsReference,
                Picture = record.Picture,
                DetailsUrl = record.DetailsUrl
            };
        }       
        
        public static AddressVm CreateViewModelContract(AddressRecord record)
        {
            if (record == null)
                return null;

            return new AddressVm
            {
                Id = record.Id,
                EntityId = record.EntityRecord.Id,
                State = VmItemState.Normal,
                Address = record.Address,
                Tip = record.Tip,
                Latitude = record.Latitude,
                Longitude = record.Longitude,
            };
        }  

        public static ContactVm CreateViewModelContract(ContactRecord record)
        {
            if (record == null)
                return null;

            return new ContactVm
            {
                Id = record.Id,
                EntityId = record.EntityRecord.Id,
                State = VmItemState.Normal,
                Type = record.Type,
                Title = record.Title,
                Contact = record.Contact
            };
        }  
    }
}