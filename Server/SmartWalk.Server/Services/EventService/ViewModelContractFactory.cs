﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class ViewModelContractFactory
    {
        public static RegionVm CreateViewModelContract(RegionRecord record)
        {
            if (record == null)
                return null;

            return new RegionVm
            {
                Id = record.Id,
                Country = record.Country,
                State = record.State,
                City = record.City
            };
        }

        public static ShowVm CreateViewModelContract(ShowRecord record) {
            if (record == null)
                return null;

            return new ShowVm {
                Id = record.Id,
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

        public static ContactVm CreateViewModelContract(ContactRecord record)
        {
            if (record == null)
                return null;

            return new ContactVm
            {
                Id = record.Id,
                EntityId = record.EntityRecord.Id,
                Type = record.Type,
                Title = record.Title,
                Contact = record.Contact
            };
        }

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
                Contacts = record.ContactRecords.Select(CreateViewModelContract).ToList()
            };
        }

        public static EventMetadataVm CreateViewModelContract(EventMetadataRecord record)
        {
            if (record == null)
                return null;

            return new EventMetadataVm
            {
                Id = record.Id,
                HostId = record.EntityRecord.Id,
                RegionId = record.RegionRecord.Id,
                HostName = record.EntityRecord.Name,
                Title = record.Title,
                CombineType = record.CombineType,
                StartTime = record.StartTime.ToString("d", CultureInfo.InvariantCulture),
                EndTime = record.EndTime.HasValue ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture) : "",
                IsMobileReady = record.IsMobileReady,
                IsWidgetReady = record.IsWidgetReady,
                Description = record.Description,
                DateCreated = record.DateCreated.ToString("d", CultureInfo.InvariantCulture),
                DateModified = record.DateModified.ToString("d", CultureInfo.InvariantCulture),
            };
        }
    }
}