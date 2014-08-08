using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.ViewModels
{
    public class AddressMapVm
    {
        public AddressMapVm(double latitude, double longitude, IList<AddressVm> addresses)
        {
            Latitude = latitude;
            Longitude = longitude;

            Addresses = addresses;
        }

        public AddressMapVm(IList<AddressVm> addresses)
        {
            var center = CalcCenter(addresses);

            Latitude = center.X;
            Longitude = center.Y;

            Addresses = addresses;
        }

        public MapType MapType { get; set; }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public IList<AddressVm> Addresses { get; private set; }

        private static PointF CalcCenter(IEnumerable<AddressVm> addresses)
        {
            var points = addresses
                .Select(address => new PointF((float)address.Latitude, (float)address.Longitude))
                .ToArray();
            return MapUtil.GetMiddleCoordinate(points);
        }
    }

    public enum MapType
    {
        Leaflet,
        Google
    }

    public static class AddressMapExtensions
    {
        #region EntityVm

        public static AddressMapVm ToAddressMapVm(this EntityVm entity)
        {
            return new AddressMapVm(entity.Addresses);
        }

        public static AddressMapVm ToAddressMapVm(this EntityVm entity, MapType maptype)
        {
            return new AddressMapVm(entity.Addresses) {MapType = maptype};
        }

        #endregion


        #region EventMNetadataVm

        public static AddressMapVm ToAddressMapVm(this EventMetadataVm metadata)
        {
            return new AddressMapVm(metadata.Latitude, metadata.Longitude, metadata.Addresses());
        }

        public static AddressMapVm ToAddressMapVm(this EventMetadataVm metadata, MapType maptype)
        {
            return new AddressMapVm(metadata.Latitude, metadata.Longitude, metadata.Addresses()) {MapType = maptype};
        }

        #endregion
    }
}