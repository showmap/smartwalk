using System.Collections.Generic;

namespace SmartWalk.Server.ViewModels
{
    public class EntityVm
    {
        public int Id { get; set; }
        public int EventMetadataId { get; set; }
        public VmItemState State { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }

        public IList<ContactVm> AllContacts { get; set; }
        public IList<AddressVm> AllAddresses { get; set; }
        public IList<ShowVm> AllShows { get; set; }

        public string DisplayName
        {
            get { return Name; }
        }

        public EntityVm() {
            AllContacts = new List<ContactVm>();
            AllAddresses = new List<AddressVm>();
            AllShows = new List<ShowVm>();
        }
    }
}