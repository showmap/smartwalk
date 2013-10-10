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
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView BottomGradientView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ContactsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionBottomConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionTopConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint HeaderHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.Placeholder ImageCellPlaceholder { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.Placeholder MapCellPlaceholder { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapXConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapYConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NavigateButton { get; set; }

		[Action ("OnContactsButtonTouchUpInside:forEvent:")]
		partial void OnContactsButtonTouchUpInside (MonoTouch.Foundation.NSObject sender, MonoTouch.UIKit.UIEvent @event);

		[Action ("OnNavigateButtonTouchUpInside:forEvent:")]
		partial void OnNavigateButtonTouchUpInside (MonoTouch.Foundation.NSObject sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (ContactsButton != null) {
				ContactsButton.Dispose ();
				ContactsButton = null;
			}

			if (NavigateButton != null) {
				NavigateButton.Dispose ();
				NavigateButton = null;
			}

			if (BottomGradientView != null) {
				BottomGradientView.Dispose ();
				BottomGradientView = null;
			}

			if (DescriptionBottomConstraint != null) {
				DescriptionBottomConstraint.Dispose ();
				DescriptionBottomConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionTopConstraint != null) {
				DescriptionTopConstraint.Dispose ();
				DescriptionTopConstraint = null;
			}

			if (HeaderHeightConstraint != null) {
				HeaderHeightConstraint.Dispose ();
				HeaderHeightConstraint = null;
			}

			if (ImageCellPlaceholder != null) {
				ImageCellPlaceholder.Dispose ();
				ImageCellPlaceholder = null;
			}

			if (ImageHeightConstraint != null) {
				ImageHeightConstraint.Dispose ();
				ImageHeightConstraint = null;
			}

			if (ImageWidthConstraint != null) {
				ImageWidthConstraint.Dispose ();
				ImageWidthConstraint = null;
			}

			if (MapCellPlaceholder != null) {
				MapCellPlaceholder.Dispose ();
				MapCellPlaceholder = null;
			}

			if (MapHeightConstraint != null) {
				MapHeightConstraint.Dispose ();
				MapHeightConstraint = null;
			}

			if (MapWidthConstraint != null) {
				MapWidthConstraint.Dispose ();
				MapWidthConstraint = null;
			}

			if (MapXConstraint != null) {
				MapXConstraint.Dispose ();
				MapXConstraint = null;
			}

			if (MapYConstraint != null) {
				MapYConstraint.Dispose ();
				MapYConstraint = null;
			}
		}
	}
}
