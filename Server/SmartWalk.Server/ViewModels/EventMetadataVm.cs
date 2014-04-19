using System.Collections.Generic;

namespace SmartWalk.Server.ViewModels
{
    public class EventMetadataVm
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Picture { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CombineType { get; set; }
        public bool IsPublic { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public string DisplayDate { get; set; }

        public EntityVm Host { get; set; }
        public IList<EntityVm> AllVenues { get; set; }
        public IList<EntityVm> AllHosts { get; set; }

        public string DisplayName
        {
            get { return Title ?? (Host != null ? Host.Name : null); }
        }

        public string DisplayPicture
        {
            get { return Picture ?? (Host != null ? Host.Picture : null); }
        }

        public EventMetadataVm() {
            AllVenues = new List<EntityVm>();
            AllHosts = new List<EntityVm>();
        }
    }
}