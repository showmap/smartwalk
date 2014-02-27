using System.Linq;
using SmartWalk.Server.DataContracts;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Extensions;
using CombineType = SmartWalk.Shared.DataContracts.CombineType;
using EntityType = SmartWalk.Shared.DataContracts.EntityType;

namespace SmartWalk.Server.Services.QueryService
{
    public static class DataContractsFactory
    {
        public static EventMetadata CreateDataContract(EventMetadataRecord record, string[] fields)
        {
            var result = new EventMetadata
                {
                    Id = record.Id
                };

            if (fields.Contains(result.GetPropertyName(p => p.Host)))
            {
                result.Host = new IReference[]{}; // TODO:
            }

            if (fields.Contains(result.GetPropertyName(p => p.Title)))
            {
                result.Title = record.Title;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Description)))
            {
                result.Description = record.Description;
            }

            if (fields.Contains(result.GetPropertyName(p => p.StartTime)))
            {
                result.StartTime = record.StartTime;
            }

            if (fields.Contains(result.GetPropertyName(p => p.EndTime)))
            {
                result.EndTime = record.EndTime;
            }

            if (fields.Contains(result.GetPropertyName(p => p.CombineType)))
            {
                result.CombineType = (CombineType)record.CombineType;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Shows)))
            {
                result.Shows = new IReference[]{}; // TODO:
            }

            return result;
        }

        public static Entity CreateDataContract(EntityRecord record, string[] fields)
        {
            var result = new Entity
                {
                    Id = record.Id
                };

            if (fields.Contains(result.GetPropertyName(p => p.Type)))
            {
                result.Type = (EntityType)record.Type;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Name)))
            {
                result.Name = record.Name;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Description)))
            {
                result.Description = record.Description;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Picture)))
            {
                result.Picture = record.Picture;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Contacts)))
            {
                result.Contacts = null; // TODO:
            }

            if (fields.Contains(result.GetPropertyName(p => p.Addresses)))
            {
                result.Addresses = null; // TODO:
            }

            return result;
        }

        public static Show CreateDataContract(ShowRecord record, string[] fields)
        {
            var result = new Show
                {
                    Id = record.Id
                };

            if (fields.Contains(result.GetPropertyName(p => p.Venue)))
            {
                result.Venue = new IReference[] { }; // TODO:
            }

            if (fields.Contains(result.GetPropertyName(p => p.IsReference)))
            {
                result.IsReference = record.IsReference;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Title)))
            {
                result.Title = record.Title;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Description)))
            {
                result.Description = record.Description;
            }

            if (fields.Contains(result.GetPropertyName(p => p.StartTime)))
            {
                result.StartTime = record.StartTime;
            }

            if (fields.Contains(result.GetPropertyName(p => p.EndTime)))
            {
                result.EndTime = record.EndTime;
            }

            if (fields.Contains(result.GetPropertyName(p => p.Picture)))
            {
                result.Picture = record.Picture;
            }

            if (fields.Contains(result.GetPropertyName(p => p.DetailsUrl)))
            {
                result.DetailsUrl = record.DetailsUrl;
            }

            return result;
        }
    }
}