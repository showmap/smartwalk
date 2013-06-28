using System;
using System.Collections.Generic;

namespace SmartWalk.Core.Model
{
    public class Venue
    {
        public Venue()
        {
        }

        public int Number { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ContactInfo Contact { get; set; }

        public IEnumerable<VenueShow> Shows { get; set; }
    }
}

