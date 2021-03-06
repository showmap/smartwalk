﻿using System.Collections.Generic;
using Newtonsoft.Json;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class EntityVm : IPicture
    {
        public EntityVm()
        {
            Contacts = new List<ContactVm>();
            Addresses = new List<AddressVm>();
            Shows = new List<ShowVm>();
        }

        public int Id { get; set; }
        public EntityType Type { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public EventEntityDetailVm EventDetail { get; set; }

        public IList<ContactVm> Contacts { get; set; }
        public IList<AddressVm> Addresses { get; set; }
        public IList<ShowVm> Shows { get; set; }

        [JsonIgnore]
        [UsedImplicitly]
        public bool Destroy { get; set; }
    }
}