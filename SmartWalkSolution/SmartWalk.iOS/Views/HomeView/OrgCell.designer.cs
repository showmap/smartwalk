// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.HomeView
{
	[Register ("OrgCell")]
	partial class OrgCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView OrgImageView { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel OrgNameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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
