// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    [Register ("VenueView")]
    partial class VenueView
    {
        [Outlet]
        MonoTouch.UIKit.UIView ProgressViewContainer { get; set; }

        [Outlet]
        SmartWalk.Client.iOS.Controls.FixedTableView VenueShowsTableView { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (ProgressViewContainer != null) {
                ProgressViewContainer.Dispose ();
                ProgressViewContainer = null;
            }

            if (VenueShowsTableView != null) {
                VenueShowsTableView.Dispose ();
                VenueShowsTableView = null;
            }
        }
    }
}
