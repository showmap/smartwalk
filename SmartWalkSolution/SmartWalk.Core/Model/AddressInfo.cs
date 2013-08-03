using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class AddressInfo
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Address { get; set; }

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