using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Labs.Protocol
{
    public class Region : IRegion
    {
        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }
    }
}