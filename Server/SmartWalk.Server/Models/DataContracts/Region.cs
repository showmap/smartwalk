using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.Models.DataContracts
{
    public class Region : IRegion
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}