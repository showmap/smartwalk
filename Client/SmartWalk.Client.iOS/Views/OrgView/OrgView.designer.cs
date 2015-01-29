// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgView
{
	[Register ("OrgView")]
	partial class OrgView
	{
		[Outlet]
        UIKit.UITableView OrgEventsTableView { get; set; }

		[Outlet]
		UIKit.UIView ProgressViewContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProgressViewTopConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (OrgEventsTableView != null) {
				OrgEventsTableView.Dispose ();
				OrgEventsTableView = null;
			}

			if (ProgressViewContainer != null) {
				ProgressViewContainer.Dispose ();
				ProgressViewContainer = null;
			}

			if (ProgressViewTopConstraint != null) {
				ProgressViewTopConstraint.Dispose ();
				ProgressViewTopConstraint = null;
			}
		}
	}
}
