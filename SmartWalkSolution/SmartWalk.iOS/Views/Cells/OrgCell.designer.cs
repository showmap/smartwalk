// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Cells
{
	[Register ("OrgCell")]
	partial class OrgCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel OrgDescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView OrgImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel OrgNameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (OrgDescriptionLabel != null) {
				OrgDescriptionLabel.Dispose ();
				OrgDescriptionLabel = null;
			}

			if (OrgImageView != null) {
				OrgImageView.Dispose ();
				OrgImageView = null;
			}

			if (OrgNameLabel != null) {
				OrgNameLabel.Dispose ();
				OrgNameLabel = null;
			}
		}
	}
}
