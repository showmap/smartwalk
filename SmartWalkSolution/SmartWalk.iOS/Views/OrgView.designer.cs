// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views
{
	[Register ("OrgView")]
	partial class OrgView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView OrgEventsTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OrgImageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (OrgImageView != null) {
				OrgImageView.Dispose ();
				OrgImageView = null;
			}

			if (OrgEventsTableView != null) {
				OrgEventsTableView.Dispose ();
				OrgEventsTableView = null;
			}
		}
	}
}
