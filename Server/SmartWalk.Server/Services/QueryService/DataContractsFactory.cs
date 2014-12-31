using System.Collections.Generic;
using System.Linq;
using Orchard.FileSystems.Media;
using Orchard.MediaProcessing.Services;
using SmartWalk.Server.Models.DataContracts;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;
using CombineType = SmartWalk.Shared.DataContracts.CombineType;
using VenueOrderType = SmartWalk.Shared.DataContracts.VenueOrderType;
using VenueTitleFormatType = SmartWalk.Shared.DataContracts.VenueTitleFormatType;
using ContactType = SmartWalk.Shared.DataContracts.ContactType;
using EntityType = SmartWalk.Shared.DataContracts.EntityType;

namespace SmartWalk.Server.Services.QueryService
{
    public static class DataContractsFactory
    {
        public static EventMetadata CreateDataContract(
            EventMetadataRecord record,
            string[] fields,
            string[] storages,
            IStorageProvider storageProvider,
            PictureSize pictureSize,
            IImageProfileManager imageProfileManager)
        {
            var result = new EventMetadata
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataHost) &&
                    record.EntityRecord != null)
                {
                    result.Host = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataTitle))
                {
                    result.Title = record.Title;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataDescription))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataPicture))
                {
                    var picture = FileUtil.GetPictureUrl(record.Picture, storageProvider);
                    result.Picture = FileUtil.GetResizedPicture(picture, pictureSize, imageProfileManager);
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataStartTime))
                {
                    result.StartTime = record.StartTime;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataEndTime))
                {
                    result.EndTime = record.EndTime;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataLatitude))
                {
                    result.Latitude = record.Latitude;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataLongitude))
                {
                    result.Longitude = record.Longitude;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataCombineType))
                {
                    result.CombineType = (CombineType)record.CombineType;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataVenueOrderType))
                {
                    result.VenueOrderType = (VenueOrderType)record.VenueOrderType;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataVenueTitleFormatType))
                {
                    result.VenueTitleFormatType = (VenueTitleFormatType)record.VenueTitleFormatType;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventMetadataShows))
                {
                    result.Shows =
                        record.ShowRecords
                              .Where(s => !s.IsDeleted)
                              .Select(mr => new Reference
                                  {
                                      Id = mr.Id,
                                      Storage = StorageKeys.SmartWalk
                                  })
                              .ToArray();
                }
            }

            return result;
        }

        public static Entity CreateDataContract(
            EntityRecord record,
            string[] fields,
            IStorageProvider storageProvider,
            PictureSize pictureSize,
            IImageProfileManager imageProfileManager)
        {
            var result = new Entity
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(QueryContext.Instance.EntityType))
                {
                    result.Type = (EntityType)record.Type;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EntityName))
                {
                    result.Name = record.Name;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EntityDescription))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EntityPicture))
                {
                    var picture = FileUtil.GetPictureUrl(record.Picture, storageProvider);
                    result.Picture = FileUtil.GetResizedPicture(picture, pictureSize, imageProfileManager);
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EntityContacts))
                {
                    result.Contacts = record.ContactRecords.Select(CreateDataContract).ToArray();
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EntityAddresses))
                {
                    result.Addresses = record.AddressRecords.Select(CreateDataContract).ToArray();
                }
            }

            return result;
        }

        public static EventVenueDetail CreateDataContract(
            EventEntityDetailRecord record,
            string[] fields,
            string[] storages)
        {
            var result = new EventVenueDetail();

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventVenueDetailVenue) &&
                    record.EntityRecord != null)
                {
                    result.Venue = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventVenueDetailSortOrder))
                {
                    result.SortOrder = record.SortOrder;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.EventVenueDetailDescription))
                {
                    result.Description = record.Description;
                }
            }

            return result;
        }

        public static Show CreateDataContract(
            ShowRecord record,
            string[] fields,
            string[] storages,
            IStorageProvider storageProvider,
            PictureSize pictureSize,
            IImageProfileManager imageProfileManager)
        {
            var result = new Show
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowVenue))
                {
                    result.Venue = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowIsReference))
                {
                    result.IsReference = record.IsReference;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowTitle))
                {
                    result.Title = record.Title;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowDescription))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowStartTime))
                {
                    result.StartTime = record.StartTime;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowEndTime))
                {
                    result.EndTime = record.EndTime;
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowPicture))
                {
                    var picture = FileUtil.GetPictureUrl(record.Picture, storageProvider);
                    result.Picture = FileUtil.GetResizedPicture(picture, pictureSize, imageProfileManager);
                }

                if (fields.ContainsIgnoreCase(QueryContext.Instance.ShowDetailsUrl))
                {
                    result.DetailsUrl = record.DetailsUrl;
                }
            }

            return result;
        }

        private static Contact CreateDataContract(ContactRecord record)
        {
            var result = new Contact
                {
                    Type = (ContactType)record.Type,
                    Title = record.Title,
                    ContactText = record.Contact
                };

            return result;
        }

        private static Address CreateDataContract(AddressRecord record)
        {
            var result = new Address
                {
                    AddressText = record.Address,
                    Latitude = record.Latitude,
                    Longitude = record.Longitude,
                    Tip = record.Tip
                };

            return result;
        }

        private static IReference[] GetEntityReferences(EntityRecord record, IEnumerable<string> storages)
        {
            var result = new[]
                {
                    new Reference
                        {
                            Id = record.Id,
                            Storage = StorageKeys.SmartWalk
                        }
                }.Union(
                    storages != null
                        ? record.EntityMappingRecords
                                .Where(emr => storages.ContainsIgnoreCase(emr.StorageRecord.StorageKey))
                                .Select(emr => new Reference
                                    {
                                        Id = emr.ExternalEntityId,
                                        Storage = emr.StorageRecord.StorageKey,
                                        Type = emr.Type
                                    })
                        : Enumerable.Empty<IReference>())
                 .ToArray();

            return result;
        }
    }
}