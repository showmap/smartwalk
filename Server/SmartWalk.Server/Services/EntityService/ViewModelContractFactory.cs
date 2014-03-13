using System;
using System.Collections.Generic;
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
                UserId = record.SmartWalkUserRecord.Id,
                Type = record.Type,
                Name = record.Name,
                Picture = record.Picture,
                Description = record.Description,
                AllContacts = record.ContactRecords.Select(CreateViewModelContract).ToList(),
                AllAddresses =  record.AddressRecords.Select(CreateViewModelContract).ToList()
            };
        }

        public static AddressVm CreateViewModelContract(AddressRecord record)
        {
            if (record == null)
                return null;

            return new AddressVm
            {
                Id = record.Id,
                State = VmItemState.Normal,
                Address = record.Address,
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