using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class RegionVm
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }

        public string DisplayName {
            get { return string.Format("{0}, {1}, {2}", Country, State, City); }
        }
    }
}