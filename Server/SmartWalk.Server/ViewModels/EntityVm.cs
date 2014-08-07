using System.Collections.Generic;
using Newtonsoft.Json;
using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class EntityVm {
        public EntityVm() {
            Contacts = new List<ContactVm>();
            Addresses = new List<AddressVm>();
            Shows = new List<ShowVm>();
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }

        public IList<ContactVm> Contacts { get; set; }
        public IList<AddressVm> Addresses { get; set; }
        public IList<ShowVm> Shows { get; set; }

        [JsonIgnore]
        [UsedImplicitly]
        public bool Destroy { get; set; }        
    }
}