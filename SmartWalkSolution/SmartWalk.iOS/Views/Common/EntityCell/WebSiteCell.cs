using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class WebSiteCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("WebSiteCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("WebSiteCell");

        public WebSiteCell(IntPtr handle) : base(handle)
        {
            //Layer.BorderColor = UIColor.Gray.CGColor;
            //Layer.BorderWidth = 1;
            Layer.CornerRadius = 3;
        }

        public new WebSiteInfo DataContext
        {
            get { return (WebSiteInfo)base.DataContext; }
            set { base.DataContext = value; }
        }

        public static WebSiteCell Create()
        {
            return (WebSiteCell)Nib.Instantiate(null, null)[0];
        }

        protected override void OnDataContextChanged()
        {
            WebSiteLabel.Text = DataContext != null 
                ? DataContext.Label ?? DataContext.URL 
                : null;
        }
    }
}