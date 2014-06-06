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
        public const int DefaultHeight = 44;
        public const int OptionsButtonWith = 44;

        public static readonly UINib Nib = UINib.FromName("OrgEventHeaderView", NSBundle.MainBundle);

        private bool _isStyleInitialized;
        private bool _isListOptionsVisible;

        public OrgEventHeaderView(IntPtr handle) : base(handle)
        {
        }

        public static OrgEventHeaderView Create()
        {
            return (OrgEventHeaderView)Nib.Instantiate(null, null)[0];
        }

        public CustomUISearchBar SearchBarControl 
        { 
            get { return SearchBar; }
        }

        public bool IsListOptionsVisible
        {
            get
            {
                return _isListOptionsVisible;
            }
            set
            {
                if (_isListOptionsVisible != value)
                {
                    _isListOptionsVisible = value;

                    SearchBarControl.IsListOptionsVisible = _isListOptionsVisible;
                    OptionsButton.SetHidden(!_isListOptionsVisible, true);
                    SetNeedsLayout();
                }
            }
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

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ShowOptionsCommand = null;
                SearchBar.Dispose();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            InitializeStyles();

            var buttonWidth =
                IsListOptionsVisible
                    ? OptionsButtonWith
                    : 0;

            SearchBar.Frame = 
                new RectangleF(
                    PointF.Empty,
                    new SizeF(
                        Frame.Width - buttonWidth, 
                        DefaultHeight));

            OptionsButton.Frame = 
                new RectangleF(
                    new PointF(
                        Frame.Width - OptionsButtonWith,
                        0),
                    new SizeF(
                        OptionsButtonWith,
                        DefaultHeight));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeStyles()
        {
            if (_isStyleInitialized || SearchBar == null) return;

            var textField = SearchBar
                .GetAllSubViews()
                .OfType<UITextField>()
                .FirstOrDefault();
            if (textField != null)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                {
                    textField.TextColor = UIColor.White;
                }
                else
                {
                    textField.Font = Theme.OrgEventHeaderFont;
                }
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            }

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