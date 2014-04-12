using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public class EntityAnnotation : MKAnnotation
    {
        private readonly Address _address;

        public EntityAnnotation(Entity entity, Address address)
        {
            Entity = entity;
            _address = address;

            Coordinate = new CLLocationCoordinate2D(_address.Latitude, _address.Longitude);
        }

        public Entity Entity { get; set; }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public override string Title
        {
            get
            {
                return Entity.Name;
            }
        }

        public override string Subtitle
        {
            get
            {
                return _address.AddressText;
            }
        }
    }
}