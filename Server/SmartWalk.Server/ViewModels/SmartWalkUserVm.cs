using System;

namespace SmartWalk.Server.ViewModels
{
    public class SmartWalkUserVm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoiginAt { get; set; }
    }
}