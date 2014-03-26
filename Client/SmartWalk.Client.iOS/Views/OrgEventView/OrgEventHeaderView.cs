using System;
using System.Linq;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventHeaderView : UIView
    {
        public const int DefaultHeight = 88;

        public static readonly UINib Nib = UINib.FromName("OrgEventHeaderView", NSBundle.MainBundle);

        private bool _isStyleInitialized;

        public OrgEventHeaderView(IntPtr handle) : base(handle)
        {
            BackgroundColor = Theme.BackgroundPatternColor;
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
                value.Height = DefaultHeight;
                base.Frame = value;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            InitializeSearchBarStyle();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeSearchBarStyle()
        {
            if (_isStyleInitialized || SearchBar == null) return;

            var textField = SearchBar.Subviews.OfType<UITextField>().FirstOrDefault();
            if (textField != null)
            {
                textField.Font = Theme.OrgEventHeaderFont;
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                SearchBar.TranslatesAutoresizingMaskIntoConstraints = true;
            }

            _isStyleInitialized = true;
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