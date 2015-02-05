using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Binding.Touch.Views;
using CoreAnimation;
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
        private static readonly ImageCache Cache = new ImageCache();

        private readonly AnimationDelay _animationDelay = new AnimationDelay();

        private MvxImageViewLoader _imageHelper;
        private MvxResizedImageViewLoader _resizedImageHelper;
        private CAGradientLayer _bottomGradient;

        private string _imageUrl;
        private bool _resizeImage;
        private bool _useCache;

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
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                BackgroundImage.Image = _useCache ? Cache.GetImage(_imageUrl) : null;

                if (BackgroundImage.Image == null)
                {
                    _animationDelay.Reset();

                    if (_resizeImage)
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

        public void Initialize(bool resizeImage = false, bool useCache = false)
        {
            _resizeImage = resizeImage;
            _useCache = useCache;

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

            BackgroundImage.BackgroundColor = BackgroundColor;
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
            if (_useCache && BackgroundImage.HasImage())
            {
                Cache.CacheImage(ImageUrl, BackgroundImage.Image);
            }

            if (BackgroundImage.HasImage() && _animationDelay.Animate)
            {
                BackgroundImage.Hidden = true;
                BackgroundImage.SetHidden(false, true);
            }
        }

        private class ImageCache
        {
            private readonly Dictionary<string, WeakReference<UIImage>> _cache = 
                new Dictionary<string, WeakReference<UIImage>>();

            public UIImage GetImage(string url)
            {
                if (_cache.ContainsKey(url))
                {
                    UIImage result;

                    if (_cache[url].TryGetTarget(out result))
                    {
                        return result;
                    }

                    _cache.Remove(url);
                }

                return null;
            }

            public void CacheImage(string url, UIImage image)
            {
                if (_cache.ContainsKey(url))
                {
                    _cache[url].SetTarget(image);
                }
                else
                {
                    _cache[url] = new WeakReference<UIImage>(image);
                }
            }
        }
    }
}