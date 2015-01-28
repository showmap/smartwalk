using System;
using CoreGraphics;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreAnimation;
using Foundation;
using UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ImageBackgroundView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ImageBackgroundView", NSBundle.MainBundle);
        private readonly AnimationDelay _animationDelay = new AnimationDelay();

        private MvxImageViewLoader _imageHelper;
        private MvxResizedImageViewLoader _resizedImageHelper;
        private CAGradientLayer _bottomGradient;

        private bool _resizeImage;

        public ImageBackgroundView(IntPtr handle) : base(handle)
        {
        }

        public static ImageBackgroundView Create()
        {
            return (ImageBackgroundView)Nib.Instantiate(null, null)[0];
        }

        public string Title
        {
            get { return TitleLabel.Text; }
            set { TitleLabel.Text = value; }
        }

        public string Subtitle
        {
            get { return SubtitleLabel.Text; }
            set
            { 
                SubtitleLabel.Text = value;
                TitleBottomGapConstraint.Constant = value != null ? 3 : 7;
            }
        }

        public string ImageUrl
        {
            get 
            {
                return _resizeImage 
                    ? _resizedImageHelper.ImageUrl 
                    : _imageHelper.ImageUrl;
            }
            set
            {
                BackgroundImage.Image = null;

                if (value != null)
                {
                    ProgressView.StartAnimating();
                }

                _animationDelay.Reset();

                if (_resizeImage)
                {
                    _resizedImageHelper.ImageUrl = value;
                }
                else
                {
                    _imageHelper.ImageUrl = value;
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

        public void Initialize(bool resizeImage = false)
        {
            _resizeImage = resizeImage;

            // removing design values set in markup
            TitleLabel.Text = null;
            SubtitleLabel.Text = null;

            InitializeImageHelper();
            InitializeStyle();
            InitializeGradient();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_bottomGradient != null)
            {
                _bottomGradient.Frame = BackgroundImage.Bounds;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeImageHelper()
        {
            if (_resizeImage)
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
        }

        private void InitializeGradient()
        {
            if (_bottomGradient == null)
            {
                _bottomGradient = new CAGradientLayer {
                    Frame = BackgroundImage.Bounds,
                    Colors = new [] { 
                        ThemeColors.ContentDarkBackground.ColorWithAlpha(0.2f).CGColor, 
                        ThemeColors.ContentDarkBackground.ColorWithAlpha(0.8f).CGColor 
                    },
                    Locations = new [] {
                        new NSNumber(0),
                        new NSNumber(1)
                    },
                    ShouldRasterize = true,
                    RasterizationScale = UIScreen.MainScreen.Scale
                };

                BackgroundImage.Layer.InsertSublayer(_bottomGradient, 0);
            }
        }

        private void OnImageChanged()
        {
            if (BackgroundImage.ProgressEnded())
            {
                ProgressView.StopAnimating();
            }

            if (BackgroundImage.HasImage() && _animationDelay.Animate)
            {
                BackgroundImage.Hidden = true;
                BackgroundImage.SetHidden(false, true);
            }
        }
    }
}