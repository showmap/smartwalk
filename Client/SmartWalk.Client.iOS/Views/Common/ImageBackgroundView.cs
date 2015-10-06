using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;
using UIKit;
using ImageState = Cirrious.MvvmCross.Plugins.DownloadCache.MvxDynamicImageHelper<UIKit.UIImage>.ImageState;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ImageBackgroundView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ImageBackgroundView", NSBundle.MainBundle);

        private readonly AnimationDelay _animationDelay = new AnimationDelay();
        private MvxResizedImageViewLoader _resizedImageHelper;
        private string _imageUrl;
        private bool _updateImageScheduled;

        public ImageBackgroundView(IntPtr handle) : base(handle)
        {
            ContentView = (UIView)Nib.Instantiate(this, null)[0];
            ContentView.Frame = Bounds;
            ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            Add(ContentView);
        }

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
                if (_imageUrl != value)
                {
                    _imageUrl = value;
                    BackgroundImage.Image = null;
                    _updateImageScheduled = true;
                    SetNeedsLayout();
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

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_updateImageScheduled)
            {
                if (BackgroundImage.Image == null && _imageUrl != null)
                {
                    _animationDelay.Reset();
                    Task.Run(() => _resizedImageHelper.ImageUrl = _imageUrl);
                }

                _updateImageScheduled = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void InitializeImageHelper()
        {
            _resizedImageHelper = 
                new MvxResizedImageViewLoader(() => BackgroundImage, () => Bounds, OnImageChanged) 
                    { UseGradient = true };
        }

        private void InitializeStyle()
        {
            TitleLabel.Font = Theme.BackgroundImageTitleTextFont;
            TitleLabel.TextColor = ThemeColors.ContentDarkText;

            SubtitleLabel.Font = Theme.BackgroundImageSubtitleTextFont;
            SubtitleLabel.TextColor = ThemeColors.Metadata;

            BackgroundColor = ThemeColors.ContentLightHighlight;
        }

        private void OnImageChanged(ImageState state)
        {
            var hasImage = BackgroundImage.HasImage(state);

            Gradient.Hidden = hasImage;

            if (hasImage && _animationDelay.Animate)
            {
                BackgroundImage.Hidden = true;
                BackgroundImage.SetHidden(false, true);
            }
        }
    }
}