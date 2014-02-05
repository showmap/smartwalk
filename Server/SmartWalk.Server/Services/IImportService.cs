using System.Collections.Generic;
using Orchard;

namespace SmartWalk.Server.Services
{
    public interface IImportService : IDependency 
    {
        void ImportXmlData(List<string> log);
    }
}