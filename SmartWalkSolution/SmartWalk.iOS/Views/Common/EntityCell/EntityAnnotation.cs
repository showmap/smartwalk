using MonoTouch.MapKit;
using MonoTouch.CoreLocation;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public class EntityAnnotation : MKAnnotation
    {
        private readonly AddressInfo _addressInfo;

        public EntityAnnotation(Entity entity, AddressInfo addressInfo)
        {
            Entity = entity;
            _addressInfo = addressInfo;
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