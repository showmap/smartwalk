// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint CollectionViewLeftConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint CollectionViewRightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UICollectionView ContactCollectionView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ContactViewWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton GoToContactButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageViewWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPageControl PageControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ScrollViewHeightConstraint { get; set; }

		[Action ("OnGoToContactButtonTouchUpInside:forEvent:")]
		partial void OnGoToContactButtonTouchUpInside (MonoTouch.UIKit.UIButton sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (CollectionViewLeftConstraint != null) {
				CollectionViewLeftConstraint.Dispose ();
				CollectionViewLeftConstraint = null;
			}

			if (CollectionViewRightConstraint != null) {
				CollectionViewRightConstraint.Dispose ();
				CollectionViewRightConstraint = null;
			}

			if (ContactCollectionView != null) {
				ContactCollectionView.Dispose ();
				ContactCollectionView = null;
			}

			if (ContactViewWidthConstraint != null) {
				ContactViewWidthConstraint.Dispose ();
				ContactViewWidthConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (GoToContactButton != null) {
				GoToContactButton.Dispose ();
				GoToContactButton = null;
			}

			if (ImageViewWidthConstraint != null) {
				ImageViewWidthConstraint.Dispose ();
				ImageViewWidthConstraint = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (PageControl != null) {
				PageControl.Dispose ();
				PageControl = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (ScrollViewHeightConstraint != null) {
				ScrollViewHeightConstraint.Dispose ();
				ScrollViewHeightConstraint = null;
			}
		}
	}
}
