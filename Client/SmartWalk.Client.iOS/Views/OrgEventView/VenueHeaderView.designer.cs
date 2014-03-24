// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
	[Register ("VenueHeaderView")]
	partial class VenueHeaderView
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel AddressLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line BottomSeparator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView GoRightImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel ImageLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ImageLabelView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel NameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NavigateOnMapButton { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line TopSeparator { get; set; }

		[Action ("OnNavigateOnMapClick:")]
		partial void OnNavigateOnMapClick (MonoTouch.UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddressLabel != null) {
				AddressLabel.Dispose ();
				AddressLabel = null;
			}

			if (BottomSeparator != null) {
				BottomSeparator.Dispose ();
				BottomSeparator = null;
			}

			if (GoRightImageView != null) {
				GoRightImageView.Dispose ();
				GoRightImageView = null;
			}

			if (ImageLabel != null) {
				ImageLabel.Dispose ();
				ImageLabel = null;
			}

			if (ImageLabelView != null) {
				ImageLabelView.Dispose ();
				ImageLabelView = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
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
