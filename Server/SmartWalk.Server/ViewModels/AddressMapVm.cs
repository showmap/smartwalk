using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.ViewModels
{
    public class AddressMapVm
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public IList<AddressVm> Addresses { get; set; }

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

        private static PointF CalcCenter(IEnumerable<AddressVm> addresses)
        {
            var points = addresses.Select(address => new PointF((float)address.Latitude, (float)address.Longitude)).ToArray();
            return MapUtil.GetMiddleCoordinate(points);
        }
    }

    public static class AddressMapExtensions
    {
        public static AddressMapVm ToAddressMapVm(this EntityVm entity)
        {
            return new AddressMapVm(entity.Addresses);
        }

        public static AddressMapVm ToAddressMapVm(this EventMetadataVm metadata)
        {
            return new AddressMapVm(metadata.Latitude, metadata.Longitude, metadata.Addresses);
        }
    }
}