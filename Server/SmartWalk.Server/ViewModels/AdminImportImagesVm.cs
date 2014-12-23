using System.Collections.Generic;

namespace SmartWalk.Server.ViewModels
{
    public class AdminImportImagesVm
    {
        public IList<ImportImageResult> Results { get; set; }
        public string Error { get; set; }
    }

    public class ImportImageResult
    {
        public ImportItemType ItemType { get; set; }
        public int ItemId { get; set; }
        public int ItemParentId { get; set; }
        public string SourceImageUrl { get; set; }
        public string TargetStoragePath { get; set; }
        public bool IsSuccessful { get; set; }
        public string Error { get; set; }
    }

    public enum ImportItemType
    {
        Venue,
        Host,
        Event,
        Show
    }
}