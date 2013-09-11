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
		SmartWalk.iOS.Controls.ExtendedCollectionView CollectionView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint CollectionViewHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomGradientView != null) {
				BottomGradientView.Dispose ();
				BottomGradientView = null;
			}

			if (CollectionView != null) {
				CollectionView.Dispose ();
				CollectionView = null;
			}

			if (CollectionViewHeightConstraint != null) {
				CollectionViewHeightConstraint.Dispose ();
				CollectionViewHeightConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}
		}
	}
}
