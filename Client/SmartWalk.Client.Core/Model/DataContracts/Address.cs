using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class Address : IAddress
    {
        public string AddressText { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Tip { get; set; }

        public override bool Equals(object obj)
        {
            var ad = obj as Address;
            if (ad != null)
            {
                return AddressText == ad.AddressText &&
                    Latitude.EqualsF(ad.Latitude) &&
                    Longitude.EqualsF(ad.Longitude) &&
                    Tip == ad.Tip;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(AddressText)
                .CombineHashCode(Latitude)
                .CombineHashCode(Longitude)
                .CombineHashCodeOrDefault(Tip);
        }
    }
}