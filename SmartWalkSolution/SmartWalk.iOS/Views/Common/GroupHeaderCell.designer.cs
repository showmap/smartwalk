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
	[Register ("GroupHeaderCell")]
	partial class GroupHeaderCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView ShadowImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ShadowImageView != null) {
				ShadowImageView.Dispose ();
				ShadowImageView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}
		}
	}
}
