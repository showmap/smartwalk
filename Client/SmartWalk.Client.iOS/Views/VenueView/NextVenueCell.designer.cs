// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.VenueView
{
	[Register ("NextVenueCell")]
	partial class NextVenueCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView ContainerView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView DownImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NextTitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DownImageView != null) {
				DownImageView.Dispose ();
				DownImageView = null;
			}

			if (NextTitleLabel != null) {
				NextTitleLabel.Dispose ();
				NextTitleLabel = null;
			}

			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}
		}
	}
}
