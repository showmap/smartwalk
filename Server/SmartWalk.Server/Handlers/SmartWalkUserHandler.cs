using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Handlers
{
    public class SmartWalkUserHandler : ContentHandler
    {
        public SmartWalkUserHandler(IRepository<SmartWalkUserRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}