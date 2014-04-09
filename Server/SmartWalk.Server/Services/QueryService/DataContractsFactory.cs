using System.Collections.Generic;
using System.Linq;
using SmartWalk.Server.Models.DataContracts;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;
using CombineType = SmartWalk.Shared.DataContracts.CombineType;
using ContactType = SmartWalk.Shared.DataContracts.ContactType;
using EntityType = SmartWalk.Shared.DataContracts.EntityType;

namespace SmartWalk.Server.Services.QueryService
{
    public static class DataContractsFactory
    {
        public static EventMetadata CreateDataContract(
            QueryContext context,
            EventMetadataRecord record,
            string[] fields,
            string[] storages)
        {
            var result = new EventMetadata
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(context.EventMetadataHost) &&
                    record.EntityRecord != null)
                {
                    result.Host = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataTitle))
                {
                    result.Title = record.Title;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataDescription))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataPicture))
                {
                    result.Picture = record.Picture;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataStartTime))
                {
                    result.StartTime = record.StartTime;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataEndTime))
                {
                    result.EndTime = record.EndTime;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataLatitude))
                {
                    result.Latitude = record.Latitude;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataLongitude))
                {
                    result.Longitude = record.Longitude;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataCombineType))
                {
                    result.CombineType = (CombineType)record.CombineType;
                }

                if (fields.ContainsIgnoreCase(context.EventMetadataShows) &&
                    record.EventMappingRecords != null)
                {
                    result.Shows =
                        record.ShowRecords
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
            QueryContext context,
            EntityRecord record,
            string[] fields)
        {
            var result = new Entity
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(context.EntityType))
                {
                    result.Type = (EntityType)record.Type;
                }

                if (fields.ContainsIgnoreCase(context.EntityName))
                {
                    result.Name = record.Name;
                }

                if (fields.ContainsIgnoreCase(context.EntityDescription))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(context.EntityPicture))
                {
                    result.Picture = record.Picture;
                }

                if (fields.ContainsIgnoreCase(context.EntityContacts))
                {
                    result.Contacts = record.ContactRecords.Select(CreateDataContract).ToArray();
                }

                if (fields.ContainsIgnoreCase(context.EntityAddresses))
                {
                    result.Addresses = record.AddressRecords.Select(CreateDataContract).ToArray();
                }
            }

            return result;
        }

        public static Show CreateDataContract(
            QueryContext context,
            ShowRecord record,
            string[] fields,
            string[] storages)
        {
            var result = new Show
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(context.ShowVenue))
                {
                    result.Venue = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(context.ShowIsReference))
                {
                    result.IsReference = record.IsReference;
                }

                if (fields.ContainsIgnoreCase(context.ShowTitle))
                {
                    result.Title = record.Title;
                }

                if (fields.ContainsIgnoreCase(context.ShowDescription))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(context.ShowStartTime))
                {
                    result.StartTime = record.StartTime;
                }

                if (fields.ContainsIgnoreCase(context.ShowEndTime))
                {
                    result.EndTime = record.EndTime;
                }

                if (fields.ContainsIgnoreCase(context.ShowPicture))
                {
                    result.Picture = record.Picture;
                }

                if (fields.ContainsIgnoreCase(context.ShowDetailsUrl))
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