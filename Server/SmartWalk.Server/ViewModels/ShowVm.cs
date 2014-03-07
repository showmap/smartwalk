﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class ShowVm
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public bool IsReference { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string Picture { get; set; }
        public string DetailsUrl { get; set; }
    }
}