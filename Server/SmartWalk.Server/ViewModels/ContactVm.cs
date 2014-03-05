using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartWalk.Server.ViewModels
{
    public class ContactVm
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Contact { get; set; }
    }
}