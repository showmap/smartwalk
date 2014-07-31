using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SmartWalk.Shared;
using System.Linq;
using System.Drawing;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.ViewModels
{
    public class EntityVm : IAddressesMapObject {
        public EntityVm() {
            Contacts = new List<ContactVm>();
            Addresses = new List<AddressVm>();
            Shows = new List<ShowVm>();
            _latitude = _longitude = 0;
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }

        public IList<ContactVm> Contacts { get; set; }
        public IList<AddressVm> Addresses { get; set; }
        public IList<ShowVm> Shows { get; set; }

        private double _latitude;

        [JsonIgnore]
        public double Latitude {
            get {
                if (Math.Abs(_latitude - 0) < 1)
                    CalcCenter();

                return _latitude;
            }
        }

        private double _longitude;

        [JsonIgnore]
        public double Longitude {
            get {
                if (Math.Abs(_longitude - 0) < 1)
                    CalcCenter();
                return _longitude;
            }
        }

        [JsonIgnore]
        [UsedImplicitly]
        public bool Destroy { get; set; }

        private void CalcCenter() {
            var points = Addresses.Select(address => new PointF((float) address.Latitude, (float) address.Longitude)).ToArray();
            var center = MapUtil.GetMiddleCoordinate(points);

            _latitude = center.X;
            _longitude = center.Y;
        }
    }
}