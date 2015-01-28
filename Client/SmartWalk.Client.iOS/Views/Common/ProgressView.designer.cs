// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common
{
	[Register ("ProgressView")]
	partial class ProgressView
	{
		[Outlet]
		UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

		[Outlet]
		UIKit.UIView LoadingView { get; set; }

		[Outlet]
		UIKit.UILabel NoDataLabel { get; set; }

		[Outlet]
		UIKit.UILabel ProgressLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadingView != null) {
				LoadingView.Dispose ();
				LoadingView = null;
			}

			if (NoDataLabel != null) {
				NoDataLabel.Dispose ();
				NoDataLabel = null;
			}

			if (ProgressLabel != null) {
				ProgressLabel.Dispose ();
				ProgressLabel = null;
			}

			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}
		}
	}
}
