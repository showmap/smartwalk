// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Common
{
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView ContactView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ContactViewWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageViewWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView LogoView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPageControl PageControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ScrollViewHeightConstraint { get; set; }

		[Action ("OnPageControlValueChanged:")]
		partial void OnPageControlValueChanged (MonoTouch.UIKit.UIPageControl sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (LogoView != null) {
				LogoView.Dispose ();
				LogoView = null;
			}

			if (ContactView != null) {
				ContactView.Dispose ();
				ContactView = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (PageControl != null) {
				PageControl.Dispose ();
				PageControl = null;
			}

			if (ScrollViewHeightConstraint != null) {
				ScrollViewHeightConstraint.Dispose ();
				ScrollViewHeightConstraint = null;
			}

			if (ImageViewWidthConstraint != null) {
				ImageViewWidthConstraint.Dispose ();
				ImageViewWidthConstraint = null;
			}

			if (ContactViewWidthConstraint != null) {
				ContactViewWidthConstraint.Dispose ();
				ContactViewWidthConstraint = null;
			}
		}
	}
}
