using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class VenueAnnotation : EntityAnnotation, IMapAnnotation
    {
        public VenueAnnotation(Venue venue, Address address)
            : base(venue.Info, address)
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
                return 0; // TODO: To support showing of letter
            }
        }

        public string Logo
        {
            get
            {
                return Venue.Info.Picture;
            }
        }
    }
}