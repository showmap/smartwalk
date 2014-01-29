using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace SmartWalk.Server.Services
{
    public interface IImportService : IDependency {
        void ImportXmlData();
    }
}