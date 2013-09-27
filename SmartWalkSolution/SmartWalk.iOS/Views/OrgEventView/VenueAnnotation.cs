using SmartWalk.Core.Model;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Utils.Map;
using SmartWalk.iOS.Views.Common.EntityCell;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class VenueAnnotation : EntityAnnotation, IMapAnnotation
    {
        public VenueAnnotation(Venue venue, AddressInfo addressInfo)
            : base(venue, addressInfo)
        {
            Venue = venue;
        }

        public Venue Venue { get; set; }

        public override string Title
        {
            get
            {
                return MapUtil.GetAnnotationTitle(0, Venue.Info.Name);
            }
        }

        public int Number
        {
            get
            {
                return Venue.Number;
            }
        }

        public string Logo
        {
            get
            {
                return Venue.Info.Logo;
            }
        }
    }
}