using System;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base.Cells;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class VenueHeaderView : TableHeaderBase
    {
        public static readonly NSString Key = new NSString("VenueHeaderContentView");

        public const float DefaultHeight = 64;

        public VenueHeaderView(IntPtr handle) : base(handle)
        {
            BackgroundView = new UIView { BackgroundColor = Theme.HeaderCellBackground };
            ContentView = VenueHeaderContentView.Create();
            ContentView.BackgroundView = BackgroundView;
            Frame = ContentView.Bounds;
            base.ContentView.RemoveSubviews();
            base.ContentView.Add(ContentView);
        }

        public new Venue DataContext
        {
            get { return (Venue)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand NavigateVenueCommand 
        { 
            get { return ContentView.NavigateVenueCommand; }
            set { ContentView.NavigateVenueCommand = value; }
        }

        public ICommand NavigateVenueOnMapCommand 
        { 
            get { return ContentView.NavigateVenueOnMapCommand; }
            set { ContentView.NavigateVenueOnMapCommand = value; }
        }

        protected new VenueHeaderContentView ContentView { get; private set; }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            ContentView.DataContext = (Venue)newContext;
        }
    }
}