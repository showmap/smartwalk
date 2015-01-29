// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    [Register ("OrgEventInfoView")]
    partial class OrgEventInfoView
	{
		[Outlet]
        UIKit.UITableView OrgEventInfoTableView { get; set; }

		[Outlet]
		UIKit.UIView ProgressViewContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProgressViewTopConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
            if (OrgEventInfoTableView != null) {
                OrgEventInfoTableView.Dispose ();
                OrgEventInfoTableView = null;
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
