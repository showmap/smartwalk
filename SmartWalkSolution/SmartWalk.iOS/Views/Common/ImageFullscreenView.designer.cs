// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.Common
{
	[Register ("ImageFullscreenController")]
	partial class ImageFullscreenView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.UIProgressImageView ImageView { get; set; }

		[Outlet]
		SmartWalk.iOS.Views.Common.ImageZoomScrollView ScrollView { get; set; }

		[Action ("OnCloseButtonTouchUpInside:forEvent:")]
		partial void OnCloseButtonTouchUpInside (MonoTouch.UIKit.UIButton sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}
		}
	}
}
