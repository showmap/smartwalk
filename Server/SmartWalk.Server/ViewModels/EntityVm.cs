using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.ViewModels
{
    public class EntityVm
    {
        public int Id { get; set; }
        public int EventMetedataId { get; set; }
        public VmItemState State { get; set; }
        public int UserId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
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