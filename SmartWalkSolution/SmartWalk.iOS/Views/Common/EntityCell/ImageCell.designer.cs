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
	[Register ("ImageCell")]
	partial class ImageCell
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ContactsButton { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.ProgressImageView ImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView Placeholder { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PlaceholderLabel { get; set; }

		[Action ("OnContactsButtonClick:forEvent:")]
		partial void OnContactsButtonClick (MonoTouch.Foundation.NSObject sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (Placeholder != null) {
				Placeholder.Dispose ();
				Placeholder = null;
			}

			if (ContactsButton != null) {
				ContactsButton.Dispose ();
				ContactsButton = null;
			}

			if (PlaceholderLabel != null) {
				PlaceholderLabel.Dispose ();
				PlaceholderLabel = null;
			}

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}
		}
	}
}
