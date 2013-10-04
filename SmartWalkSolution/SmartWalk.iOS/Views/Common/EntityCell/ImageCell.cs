using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class ImageCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("ImageCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ImageCell");

        private readonly MvxImageViewLoader _imageHelper;

        private UITapGestureRecognizer _imageTapGesture;
        private bool _isShadowHidden;

        public ImageCell(IntPtr handle) : base (handle)
        {
            _imageHelper = new MvxImageViewLoader(
                () => ImageView,
                () => {
                    if (_imageHelper.ImageUrl != null && 
                        ImageView.Image != null)
                    {
                        ImageView.StopProgress();
                    }
                    else if (_imageHelper.ImageUrl == null)
                    {
                        ImageView.StopProgress();
                    }
                    else
                    {
                        ImageView.StartProgress();
                    }
                });
        }

        public new EntityInfo DataContext
        {
            get { return (EntityInfo)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand ShowContactsViewCommand { get; set; }
        public ICommand NavigateWebSiteCommand { get; set; }


        public bool IsShadowHidden
        {
            get
            {
                return _isShadowHidden;
            }
            set
            {
                if (_isShadowHidden != value)
                {
                    _isShadowHidden = value;

                    if (ShadowImageView != null)
                    {
                        ShadowImageView.Hidden = _isShadowHidden;
                    }
                }
            }
        }

        public static ImageCell Create()
        {
            return (ImageCell)Nib.Instantiate(null, null)[0];
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ShowImageFullscreenCommand = null;
                ShowContactsViewCommand = null;
                NavigateWebSiteCommand = null;

                DisposeGestures();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        protected override void OnInitialize()
        {
            InitializeGestures();

            ImageView.ActivityIndicatorViewStyle = 
                UIActivityIndicatorViewStyle.White;

            ShadowImageView.Hidden = IsShadowHidden;
            ShadowImageView.Image = Theme.ShadowImage;
        }

        protected override void OnDataContextChanged()
        {
            ImageView.Image = null;
            _imageHelper.ImageUrl = null; // we have to reset it, since Image is reset too

            _imageHelper.ImageUrl = DataContext != null 
                ? DataContext.Logo : null;
        }

        private void InitializeGestures()
        {
            _imageTapGesture = new UITapGestureRecognizer(() => {
                if (ShowImageFullscreenCommand != null &&
                    ShowImageFullscreenCommand.CanExecute(DataContext.Logo))
                {
                    ShowImageFullscreenCommand.Execute(DataContext.Logo);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            ImageView.AddGestureRecognizer(_imageTapGesture);
        }

        private void DisposeGestures()
        {
            if (_imageTapGesture != null)
            {
                ImageView.RemoveGestureRecognizer(_imageTapGesture);
                _imageTapGesture.Dispose();
                _imageTapGesture = null;
            }
        }

        partial void OnContactsButtonClick(NSObject sender, UIEvent @event)
        {
            if (ShowContactsViewCommand != null &&
                ShowContactsViewCommand.CanExecute(DataContext))
            {
                ShowContactsViewCommand.Execute(DataContext);
            }
        }
    }
}