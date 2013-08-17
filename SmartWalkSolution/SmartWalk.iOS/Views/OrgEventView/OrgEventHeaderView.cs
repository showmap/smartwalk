using System;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class OrgEventHeaderView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("OrgEventHeaderView", NSBundle.MainBundle);

        public OrgEventHeaderView(IntPtr handle) : base(handle)
        {
        }

        public static OrgEventHeaderView Create()
        {
            return (OrgEventHeaderView)Nib.Instantiate(null, null)[0];
        }

        public UISearchBar SearchBarControl 
        { 
            get { return SearchBar; }
        }

        public ICommand GroupByLocationCommand { get; set; }

        public override RectangleF Frame
        {
            set
            {
                // HACK reseting height to fix weird view size on rotation
                value.Height = 82;
                base.Frame = value;
            }
        }

        partial void OnGroupByLocationTouchUpInside(UISwitch sender, UIEvent @event)
        {
            if (GroupByLocationCommand != null &&
                GroupByLocationCommand.CanExecute(sender.On))
            {
                GroupByLocationCommand.Execute(sender.On);
            }
        }
    }
}