using System;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventHeaderView : UIView
    {
        public const int DefaultHeight = 88;
        private const int OptionsButtonWith = 44;

        public static readonly UINib Nib = UINib.FromName("OrgEventHeaderView", NSBundle.MainBundle);

        private bool _isStyleInitialized;

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

        public string Title
        {
            get { return TitleLabel.Text; }
            set { TitleLabel.Text = value; }
        }

        public ICommand ShowOptionsCommand { get; set; }

        public override RectangleF Frame
        {
            set
            {
                // HACK reseting height to fix weird view size of SerachBar on rotation
                value.Height = DefaultHeight;
                base.Frame = value;
            }
        }

        // HACK To support correct Frame after OrgEventSearchDelegate.DidEndSearch
        public static RectangleF GetSearchBarFrame(UISearchBar searchBar)
        {
            var result =
                    new RectangleF(
                        new PointF(0, DefaultHeight / 2),
                        new SizeF(
                            searchBar.Frame.Width - OptionsButtonWith, 
                            DefaultHeight / 2));
            return result;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            InitializeStyles();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeStyles()
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

            TitleLabel.Font = Theme.OrgEventHeaderFont;
            TitleLabel.TextColor = Theme.CellText;

            OptionsButton.SetImage(ThemeIcons.ListOptions, UIControlState.Normal);

            _isStyleInitialized = true;
        }

        partial void OnOptionsButtonTouchUpInside(UIButton sender)
        {
            if (ShowOptionsCommand != null &&
                ShowOptionsCommand.CanExecute(true))
            {
                ShowOptionsCommand.Execute(true);
            }
        }
    }
}