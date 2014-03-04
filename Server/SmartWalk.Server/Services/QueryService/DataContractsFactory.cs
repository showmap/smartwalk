using System.Collections.Generic;
using System.Linq;
using SmartWalk.Server.Models.DataContracts;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Extensions;
using CombineType = SmartWalk.Shared.DataContracts.CombineType;
using ContactType = SmartWalk.Shared.DataContracts.ContactType;
using EntityType = SmartWalk.Shared.DataContracts.EntityType;
using SmartWalk.Server.Extensions;

namespace SmartWalk.Server.Services.QueryService
{
    public static class DataContractsFactory
    {
        public static EventMetadata CreateDataContract(
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
                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Region)))
                {
                    result.Region = CreateDataContract(record.RegionRecord);
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Host)) &&
                    record.EntityRecord != null)
                {
                    result.Host = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Title)))
                {
                    result.Title = record.Title;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Description)))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.StartTime)))
                {
                    result.StartTime = record.StartTime;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.EndTime)))
                {
                    result.EndTime = record.EndTime;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.CombineType)))
                {
                    result.CombineType = (CombineType)record.CombineType;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Shows)) &&
                    record.EventMappingRecords != null)
                {
                    result.Shows =
                        record.EventMappingRecords
                              .Select(mr => new Reference
                                  {
                                      Id = mr.ShowRecord_Id,
                                      Storage = mr.StorageRecord.StorageKey
                                  })
                              .ToArray();
                }
            }

            return result;
        }

        public static Entity CreateDataContract(
            EntityRecord record,
            string[] fields)
        {
            var result = new Entity
                {
                    Id = record.Id
                };

            if (fields != null)
            {
                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Type)))
                {
                    result.Type = (EntityType)record.Type;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Name)))
                {
                    result.Name = record.Name;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Description)))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Picture)))
                {
                    result.Picture = record.Picture;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Contacts)))
                {
                    result.Contacts = record.ContactRecords.Select(CreateDataContract).ToArray();
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Addresses)))
                {
                    result.Addresses = record.AddressRecords.Select(CreateDataContract).ToArray();
                }
            }

            return result;
        }

        public static Show CreateDataContract(
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
                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Venue)))
                {
                    result.Venue = GetEntityReferences(record.EntityRecord, storages);
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.IsReference)))
                {
                    result.IsReference = record.IsReference;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Title)))
                {
                    result.Title = record.Title;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Description)))
                {
                    result.Description = record.Description;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.StartTime)))
                {
                    result.StartTime = record.StartTime;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.EndTime)))
                {
                    result.EndTime = record.EndTime;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.Picture)))
                {
                    result.Picture = record.Picture;
                }

                if (fields.ContainsIgnoreCase(result.GetPropertyName(p => p.DetailsUrl)))
                {
                    result.DetailsUrl = record.DetailsUrl;
                }
            }

            return result;
        }

        private static Region CreateDataContract(RegionRecord record)
        {
            var result = new Region
                {
                    Country = record.Country,
                    State = record.State,
                    City = record.City
                };

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
                    Longitude = record.Longitude
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