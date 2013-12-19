using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.OrgEventView
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