// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
	[Register ("VenueHeaderContentView")]
	partial class VenueHeaderContentView
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.Line BottomSeparator { get; set; }

		[Outlet]
		UIKit.UIImageView GoRightImageView { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel NameLabel { get; set; }

		[Outlet]
		UIKit.UIButton NavigateOnMapButton { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line TopSeparator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomSeparator != null) {
				BottomSeparator.Dispose ();
				BottomSeparator = null;
			}

			if (GoRightImageView != null) {
				GoRightImageView.Dispose ();
				GoRightImageView = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (NavigateOnMapButton != null) {
				NavigateOnMapButton.Dispose ();
				NavigateOnMapButton = null;
			}

			if (TopSeparator != null) {
				TopSeparator.Dispose ();
				TopSeparator = null;
			}
		}
	}
}
