using System;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    public partial class NextVenueCell : TableCellBase
    {
        public static readonly UINib Nib = UINib.FromName("NextVenueCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("NextVenueCell");

        public const float DefaultHeight = 100;

        private UITapGestureRecognizer _cellTapGesture;

        public NextVenueCell(IntPtr handle) : base(handle)
        {
        }

        public static NextVenueCell Create()
        {
            return (NextVenueCell)Nib.Instantiate(null, null)[0];
        }

        public ICommand ShowNextEntityCommand { get; set; }
            
        public new string DataContext
        {
            get { return (string)base.DataContext; }
            set { base.DataContext = value; }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ShowNextEntityCommand = null;

                DisposeGestures();
            }
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeStyle();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            NextTitleLabel.Text = string.Format(Localization.NextPattern, DataContext);
        }

        private void InitializeGestures()
        {
            _cellTapGesture = new UITapGestureRecognizer(() => {
                if (ShowNextEntityCommand != null &&
                    ShowNextEntityCommand.CanExecute(null))
                {
                    ShowNextEntityCommand.Execute(null);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            ContainerView.AddGestureRecognizer(_cellTapGesture);
        }

        private void DisposeGestures()
        {
            if (_cellTapGesture != null)
            {
                ContainerView.RemoveGestureRecognizer(_cellTapGesture);
                _cellTapGesture.Dispose();
                _cellTapGesture = null;
            }
        }

        private void InitializeStyle()
        {
            BackgroundColor = UIColor.Clear;

            DownImageView.Image = ThemeIcons.NextEntity;

            NextTitleLabel.Font = Theme.NextEntityFont;
            NextTitleLabel.TextColor = Theme.CellText;
        }
    }
}