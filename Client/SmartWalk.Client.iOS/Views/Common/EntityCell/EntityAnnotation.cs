using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public class EntityAnnotation : MKAnnotation
    {
        private readonly AddressInfo _addressInfo;

        public EntityAnnotation(Entity entity, AddressInfo addressInfo)
        {
            Entity = entity;
            _addressInfo = addressInfo;

            Coordinate = new CLLocationCoordinate2D(_addressInfo.Latitude, _addressInfo.Longitude);
        }

        public Entity Entity { get; set; }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public override string Title
        {
            get
            {
                return Entity.Info.Name;
            }
        }

        public override string Subtitle
        {
            get
            {
                return _addressInfo.Address;
            }
        }
    }
}