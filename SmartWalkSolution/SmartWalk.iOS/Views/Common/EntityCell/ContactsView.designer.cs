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
	[Register ("ContactsView")]
	partial class ContactsView
	{
		[Outlet]
		MonoTouch.UIKit.UIView BackgroundView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint CollectionBottomConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint CollectionTopConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UICollectionView CollectionView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint PlaceholderBottomConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint PlaceholderTopConstraint { get; set; }

		[Action ("OnCloseButtonClick:forEvent:")]
		partial void OnCloseButtonClick (MonoTouch.Foundation.NSObject sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}

			if (PlaceholderTopConstraint != null) {
				PlaceholderTopConstraint.Dispose ();
				PlaceholderTopConstraint = null;
			}

			if (PlaceholderBottomConstraint != null) {
				PlaceholderBottomConstraint.Dispose ();
				PlaceholderBottomConstraint = null;
			}

			if (CollectionBottomConstraint != null) {
				CollectionBottomConstraint.Dispose ();
				CollectionBottomConstraint = null;
			}

			if (CollectionTopConstraint != null) {
				CollectionTopConstraint.Dispose ();
				CollectionTopConstraint = null;
			}
		}
	}
}
