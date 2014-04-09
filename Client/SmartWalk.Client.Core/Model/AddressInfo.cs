using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public class AddressInfo : ISearchable
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Address { get; set; }

        public string SearchableText
        {
            get
            {
                return Address != null ? " " + Address : string.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as AddressInfo;
            if (info != null)
            {
                return Latitude == info.Latitude &&
                    Longitude == info.Longitude &&
                        Address == Address;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(Latitude)
                    .CombineHashCode(Longitude)
                    .CombineHashCodeOrDefault(Address);
        }
    }
}