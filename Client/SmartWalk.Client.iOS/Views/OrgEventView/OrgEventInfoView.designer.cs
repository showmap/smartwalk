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
    [Register ("OrgEventInfoView")]
    partial class OrgEventInfoView
	{
		[Outlet]
        SmartWalk.Client.iOS.Controls.FixedTableView OrgEventInfoTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ProgressViewContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ProgressViewTopConstraint { get; set; }
		
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
