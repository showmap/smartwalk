// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views
{
	[Register ("VenueView")]
	partial class VenueView
	{
		[Outlet]
		MonoTouch.UIKit.UITableView VenueShowsTableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (VenueShowsTableView != null) {
				VenueShowsTableView.Dispose ();
				VenueShowsTableView = null;
			}
		}
	}
}
