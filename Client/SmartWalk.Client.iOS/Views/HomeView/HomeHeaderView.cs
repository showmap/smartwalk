using System;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class HomeHeaderView : UICollectionReusableView, IMvxBindable
    {
        public static readonly UINib Nib = UINib.FromName("HomeHeaderView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("HomeHeaderView");

        public const float DefaultHeight = 44;

        private UITapGestureRecognizer _viewTapGesture;

        public HomeHeaderView(IntPtr handle) : base(handle)
        {
            this.CreateBindingContext();
        }

        public IMvxBindingContext BindingContext { get; set; }

        public HomeViewModel DataContext
        {
            get { return (HomeViewModel)BindingContext.DataContext; }
            set { BindingContext.DataContext = value; }
        }

        object IMvxDataConsumer.DataContext
        {
            get { return BindingContext.DataContext; }
            set { BindingContext.DataContext = value; }
        }

        public static HomeHeaderView Create()
        {
            return (HomeHeaderView)Nib.Instantiate(null, null)[0];
        }

        public void Initialize()
        {
            this.CreateBinding(TitleLabel)
                .To<HomeViewModel>(vm => vm.Title)
                .Apply();

            InitializeStyle();
            InitializeGestures();
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                DisposeGestures();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BindingContext.ClearAllBindings();
            }

            ConsoleUtil.LogDisposed(this);
            base.Dispose(disposing);
        }

        private void InitializeStyle()
        {
            BackgroundColor = UIColor.White;

            TitleLabel.Font = Theme.HomeHeaderFont;
            TitleLabel.TextColor = Theme.CellText;
        }

        private void InitializeGestures()
        {
            _viewTapGesture = new UITapGestureRecognizer(() => {
                if (DataContext != null &&
                    DataContext.ShowTitleDetailsCommand != null &&
                    DataContext.ShowTitleDetailsCommand.CanExecute(null))
                {
                    DataContext.ShowTitleDetailsCommand.Execute(null);
                }
            }) {
                NumberOfTapsRequired = (uint)1
            };

            AddGestureRecognizer(_viewTapGesture);
        }

        private void DisposeGestures()
        {
            if (_viewTapGesture != null)
            {
                RemoveGestureRecognizer(_viewTapGesture);
                _viewTapGesture.Dispose();
                _viewTapGesture = null;
            }
        }
    }
}