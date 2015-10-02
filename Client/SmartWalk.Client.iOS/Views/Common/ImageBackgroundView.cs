using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreGraphics;
using Foundation;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;
using UIKit;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ImageBackgroundView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ImageBackgroundView", NSBundle.MainBundle);

        private readonly AnimationDelay _animationDelay = new AnimationDelay();

        private MvxImageViewLoader _imageHelper;
        private MvxResizedImageViewLoader _resizedImageHelper;

        private string _imageUrl;

        public ImageBackgroundView(IntPtr handle) : base(handle)
        {
            ContentView = (UIView)Nib.Instantiate(this, null)[0];
            ContentView.Frame = Bounds;
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            Add(ContentView);
        }

        public bool ResizeImage { get; set; }

        public string Title
        {
            get { return TitleLabel.Text; }
            set { TitleLabel.Text = value; }
        }

        public string Subtitle
        {
            get { return SubtitleLabel.Text; }
            set { SubtitleLabel.Text = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                BackgroundImage.Image = null;

                if (BackgroundImage.Image == null)
                {
                    _animationDelay.Reset();

                    if (ResizeImage)
                    {
                        _resizedImageHelper.ImageUrl = _imageUrl;
                    }
                    else
                    {
                        _imageHelper.ImageUrl = _imageUrl;
                    }
                }
            }
        }

        public override CGRect Frame
        {
            get { return base.Frame; }
            set
            {
                base.Frame = value;

                // Making sure that it has proper frame for loading a resized image
                if (BackgroundImage != null)
                {
                    BackgroundImage.Frame = Bounds;
                }
            }
        }

        public override UIColor BackgroundColor
        {
            get { return base.BackgroundColor; }
            set
            {
                base.BackgroundColor = value;

                if (BackgroundImage != null)
                {
                    BackgroundImage.BackgroundColor = value;
                }
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            // removing design values set in markup
            TitleLabel.Text = null;
            SubtitleLabel.Text = null;

            InitializeImageHelper();
            InitializeStyle();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeImageHelper()
        {
            if (ResizeImage)
            {
                _resizedImageHelper = 
                    new MvxResizedImageViewLoader(() => BackgroundImage, OnImageChanged);
            }
            else
            {
                _imageHelper = 
                    new MvxImageViewLoader(() => BackgroundImage, OnImageChanged);
                _imageHelper.DefaultImagePath = Theme.DefaultImagePath;
                _imageHelper.ErrorImagePath = Theme.ErrorImagePath;
            }
        }

        private void InitializeStyle()
        {
            TitleLabel.Font = Theme.BackgroundImageTitleTextFont;
            TitleLabel.TextColor = ThemeColors.ContentDarkText;

            SubtitleLabel.Font = Theme.BackgroundImageSubtitleTextFont;
            SubtitleLabel.TextColor = ThemeColors.Metadata;

            BackgroundImage.BackgroundColor = BackgroundColor;
        }

        private void OnImageChanged()
        {
            if (BackgroundImage.HasImage() && _animationDelay.Animate)
            {
                BackgroundImage.Hidden = true;
                BackgroundImage.SetHidden(false, true);
            }
        }
    }
}