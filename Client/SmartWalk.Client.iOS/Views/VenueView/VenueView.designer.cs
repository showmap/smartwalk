// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.VenueView
{
	[Register ("VenueView")]
	partial class VenueView
	{
		[Outlet]
		SmartWalk.Client.iOS.Views.Common.ProgressView ProgressView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProgressViewTopConstraint { get; set; }

		[Outlet]
		UIKit.UITableView VenueShowsTableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProgressView != null) {
				ProgressView.Dispose ();
				ProgressView = null;
			}

			if (ProgressViewTopConstraint != null) {
				ProgressViewTopConstraint.Dispose ();
				ProgressViewTopConstraint = null;
			}

			if (VenueShowsTableView != null) {
				VenueShowsTableView.Dispose ();
				VenueShowsTableView = null;
			}
		}
	}
}
