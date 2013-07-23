// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.HomeView
{
	[Register ("HomeView")]
	partial class HomeView
	{
		[Outlet]
		MonoTouch.UIKit.UICollectionView OrgCollectionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (OrgCollectionView != null) {
				OrgCollectionView.Dispose ();
				OrgCollectionView = null;
			}
		}
	}
}
