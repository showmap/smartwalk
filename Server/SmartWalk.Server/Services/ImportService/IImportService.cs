using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.ImportService
{
    public interface IImportService : IDependency 
    {
        List<string> ImportXmlData();
        List<ImportImageResult> ImportImages(ImportItemType type);
    }
}