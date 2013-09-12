using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class ImageCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("ImageCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("ImageCell");

        private readonly MvxImageViewLoader _imageHelper;

        private UITapGestureRecognizer _imageTapGesture;

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
        public ICommand NavigateWebSiteCommand { get; set; }

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
        }

        protected override void OnDataContextChanged()
        {
            ImageView.Image = null;
            _imageHelper.ImageUrl = null;

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
        }
    }
}