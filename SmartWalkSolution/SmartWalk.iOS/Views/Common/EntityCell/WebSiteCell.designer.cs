// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
	[Register ("WebSiteCell")]
	partial class WebSiteCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel WebSiteLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WebSiteLabel != null) {
				WebSiteLabel.Dispose ();
				WebSiteLabel = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}
		}
	}
}
