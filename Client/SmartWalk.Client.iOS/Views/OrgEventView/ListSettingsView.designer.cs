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
	[Register ("ListSettingsView")]
	partial class ListSettingsView
	{
		[Outlet]
		UIKit.UIToolbar BackgroundView { get; set; }

		[Outlet]
		UIKit.UIButton FavoritesButton { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }

		[Outlet]
		UIKit.UILabel SortByLabel { get; set; }

		[Outlet]
		UIKit.UISegmentedControl SortBySegments { get; set; }

		[Action ("OnFavoritesClick:")]
		partial void OnFavoritesClick (UIKit.UIButton sender);

		[Action ("OnSortBySegmentsValueChanged:")]
		partial void OnSortBySegmentsValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (FavoritesButton != null) {
				FavoritesButton.Dispose ();
				FavoritesButton = null;
			}

			if (SortByLabel != null) {
				SortByLabel.Dispose ();
				SortByLabel = null;
			}

			if (SortBySegments != null) {
				SortBySegments.Dispose ();
				SortBySegments = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}
		}
	}
}
