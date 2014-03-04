using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Labs.Api.DataContracts
{
    public class Region : IRegion
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}
