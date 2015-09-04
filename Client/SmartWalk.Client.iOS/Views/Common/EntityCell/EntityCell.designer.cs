// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		UIKit.NSLayoutConstraint DescriptionBottomConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DescriptionTopConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint HeaderHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Views.Common.ImageBackgroundView ImageBackground { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Views.Common.EntityCell.MapCell MapCell { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MapHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MapWidthConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MapXConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MapYConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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

			if (ImageBackground != null) {
				ImageBackground.Dispose ();
				ImageBackground = null;
			}

			if (ImageHeightConstraint != null) {
				ImageHeightConstraint.Dispose ();
				ImageHeightConstraint = null;
			}

			if (ImageWidthConstraint != null) {
				ImageWidthConstraint.Dispose ();
				ImageWidthConstraint = null;
			}

			if (MapCell != null) {
				MapCell.Dispose ();
				MapCell = null;
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
