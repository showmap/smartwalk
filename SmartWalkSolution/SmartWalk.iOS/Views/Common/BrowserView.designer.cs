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
	[Register ("BrowserView")]
	partial class BrowserView
	{
		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem ActionButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem BackButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar BottomToolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem ForwardButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem ProgressButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIWebView WebView { get; set; }

		[Action ("OnActionButtonClick:")]
		partial void OnActionButtonClick (MonoTouch.Foundation.NSObject sender);

		[Action ("OnBackButtonClick:")]
		partial void OnBackButtonClick (MonoTouch.Foundation.NSObject sender);

		[Action ("OnForwardButtonClick:")]
		partial void OnForwardButtonClick (MonoTouch.Foundation.NSObject sender);

		[Action ("OnRefreshButtonClick:")]
		partial void OnRefreshButtonClick (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackButton != null) {
				BackButton.Dispose ();
				BackButton = null;
			}

			if (BottomToolbar != null) {
				BottomToolbar.Dispose ();
				BottomToolbar = null;
			}

			if (ForwardButton != null) {
				ForwardButton.Dispose ();
				ForwardButton = null;
			}

			if (ProgressButton != null) {
				ProgressButton.Dispose ();
				ProgressButton = null;
			}

			if (ActionButton != null) {
				ActionButton.Dispose ();
				ActionButton = null;
			}

			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}
		}
	}
}
