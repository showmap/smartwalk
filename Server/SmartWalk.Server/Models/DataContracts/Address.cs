using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.Models.DataContracts
{
    public class Address : IAddress
    {
        public string AddressText { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Tip { get; set; }
    }
}