using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Windows.Input;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class GroupByCell : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("GroupByCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("GroupByCell");

        public GroupByCell(IntPtr handle) : base (handle)
        {
        }

        public static GroupByCell Create()
        {
            return (GroupByCell)Nib.Instantiate(null, null)[0];
        }

        public ICommand GroupByLocationCommand { get; set; }

        partial void OnGroupBySwitchTouchUpInside(UISwitch sender, UIEvent @event)
        {
            if (GroupByLocationCommand != null &&
                GroupByLocationCommand.CanExecute(sender.On))
            {
                GroupByLocationCommand.Execute(sender.On);
            }
        }
    }
}

