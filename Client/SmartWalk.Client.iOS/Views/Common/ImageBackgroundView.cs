using System;
using System.Drawing;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ImageBackgroundView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ImageBackgroundView", NSBundle.MainBundle);

        private const int Gap = 10;
        private const int ButtonWidth = 40;
        private const int SubtitleHeight = 50;

        private MvxImageViewLoader _imageHelper;
        private MvxResizedImageViewLoader _resizedImageHelper;
        private UITapGestureRecognizer _imageTapGesture;
        private UITapGestureRecognizer _titleTapGesture;
        private UITapGestureRecognizer _subtitleTapGesture;
        private CAGradientLayer _bottomGradient;

        private ICommand _showImageFullscreenCommand;
        private ICommand _showSubtitleContentCommand;

        private bool _resizeImage;

        public ImageBackgroundView(IntPtr handle) : base(handle)
        {
        }

        public static ImageBackgroundView Create()
        {
            return (ImageBackgroundView)Nib.Instantiate(null, null)[0];
        }

        public ICommand ShowImageFullscreenCommand
        {
            get { return _showImageFullscreenCommand; }
            set
            {
                _showImageFullscreenCommand = value;
                InitializeGestures();
            }
        }

        public ICommand ShowSubtitleContentCommand
        {
            get { return _showSubtitleContentCommand; }
            set
            {
                _showSubtitleContentCommand = value;
                InitializeGestures();
            }
        }

        public string Uptitle
        {
            get { return UptitleLabel.Text; }
            set { UptitleLabel.Text = value; }
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

        public UIImage SubtitleButtonImage
        {
            get { return SubtitleButton.CurrentImage; }
            set { SubtitleButton.SetImage(value, UIControlState.Normal); }
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
                    BackgroundImage.StartProgress();
                }

                if (_resizeImage)
                {
                    _resizedImageHelper.ImageUrl = value;
                }
                else
                {
                    _imageHelper.ImageUrl = value;
                }

                SetNeedsLayout();
            }
        }

        public override RectangleF Frame
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

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ShowImageFullscreenCommand = null;
                ShowSubtitleContentCommand = null;

                DisposeGestures();
            }
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            if (SubtitleButtonImage == null)
            {
                SubtitleButton.Hidden = true;
                SubtitleLeftConstraint.Constant = Gap;
            }
            else
            {
                SubtitleButton.Hidden = false;
                SubtitleLeftConstraint.Constant = ButtonWidth;
            }

            if (Subtitle == null)
            {
                SubtitleHeightConstraint.Constant = 0;
            }
            else
            {
                SubtitleHeightConstraint.Constant = 
                    (float)Math.Ceiling(Theme.ImageSubtitleTextFont.LineHeight * 2);
            }
        }

        public void Initialize(bool resizeImage = false)
        {
            _resizeImage = resizeImage;

            // removing design values set in markup
            UptitleLabel.Text = null;
            TitleLabel.Text = null;
            SubtitleLabel.Text = null;

            InitializeImageHelper();
            InitializeStyle();
            InitializeBottomGradientState();
            InitializeGestures();

            BackgroundImage.ActivityIndicatorViewStyle = 
                UIActivityIndicatorViewStyle.White;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateGradientState();

            if (_bottomGradient != null)
            {
                _bottomGradient.Frame = GradientPlaceholder.Bounds;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        partial void OnSubtitleButtonTouchInsideUp(UIButton sender)
        {
            if (ShowSubtitleContentCommand != null &&
                ShowSubtitleContentCommand.CanExecute(null))
            {
                ShowSubtitleContentCommand.Execute(null);
            }
        }

        private void InitializeImageHelper()
        {
            if (_resizeImage)
            {
                _resizedImageHelper = 
                    new MvxResizedImageViewLoader(
                        () => BackgroundImage,
                        UpdateGradientState);
            }
            else
            {
                _imageHelper = 
                    new MvxImageViewLoader(
                        () => BackgroundImage,
                        UpdateGradientState);
                _imageHelper.DefaultImagePath = Theme.DefaultImagePath;
                _imageHelper.ErrorImagePath = Theme.ErrorImagePath;
            }
        }

        private void InitializeStyle()
        {
            BackgroundColor = Theme.HeaderCellBackground;

            TitleLabel.Font = Theme.ImageTitleTextFont;
            TitleLabel.TextColor = Theme.ImageTitleText;
            TitleLabel.ShadowColor = Theme.ImageTextShadow;

            UptitleLabel.Font = Theme.ImageSubtitleTextFont;
            UptitleLabel.TextColor = Theme.ImageSubtitleText;
            UptitleLabel.ShadowColor = Theme.ImageTextShadow;
            UptitleLabel.BackgroundColor = Theme.ImageUptitle;

            SubtitleLabel.Font = Theme.ImageSubtitleTextFont;
            SubtitleLabel.TextColor = Theme.ImageSubtitleText;
            SubtitleLabel.ShadowColor = Theme.ImageTextShadow;
        }

        private void InitializeBottomGradientState()
        {
            if (_bottomGradient == null)
            {
                _bottomGradient = new CAGradientLayer {
                    Frame = GradientPlaceholder.Bounds,
                    Colors = new [] { 
                        Theme.ImageGradient.ColorWithAlpha(0f).CGColor, 
                        Theme.ImageGradient.ColorWithAlpha(0.1f).CGColor, 
                        Theme.ImageGradient.ColorWithAlpha(0.6f).CGColor 
                    },
                    Locations = new [] {
                        new NSNumber(0),
                        new NSNumber(0.2),
                        new NSNumber(1)
                    },
                };
            }
        }

        private void ShowGradient()
        {
            if (GradientPlaceholder != null &&
                GradientPlaceholder.Layer != null &&
                _bottomGradient != null &&
                Array.IndexOf(GradientPlaceholder.Layer.Sublayers, _bottomGradient) < 0)
            {
                GradientPlaceholder.Layer.InsertSublayer(_bottomGradient, 0);
            }
        }

        private void HideGradient()
        {
            if (_bottomGradient != null)
            {
                _bottomGradient.RemoveFromSuperLayer();
            }
        }

        private void UpdateGradientState()
        {
            if (BackgroundImage != null &&
                (BackgroundImage.Image == null ||
                    BackgroundImage.Image.Size == Theme.DefaultImageSize ||
                    BackgroundImage.Image.Size == Theme.ErrorImageSize))
            {
                HideGradient();
            }
            else
            {
                ShowGradient();
            }
        }

        private void InitializeGestures()
        {
            if (_imageTapGesture == null &&
                ShowImageFullscreenCommand != null)
            {
                _imageTapGesture = new UITapGestureRecognizer(() =>
                    {
                        if (ShowImageFullscreenCommand != null &&
                            ShowImageFullscreenCommand.CanExecute(ImageUrl))
                        {
                            ShowImageFullscreenCommand.Execute(ImageUrl);
                        }
                    }) {
                        NumberOfTouchesRequired = (uint)1,
                        NumberOfTapsRequired = (uint)1
                    };

                BackgroundImage.AddGestureRecognizer(_imageTapGesture);

                _titleTapGesture = new UITapGestureRecognizer(() =>
                    {
                        if (ShowImageFullscreenCommand != null &&
                            ShowImageFullscreenCommand.CanExecute(ImageUrl))
                        {
                            ShowImageFullscreenCommand.Execute(ImageUrl);
                        }
                    }) {
                        NumberOfTouchesRequired = (uint)1,
                        NumberOfTapsRequired = (uint)1
                    };


                TitleLabel.AddGestureRecognizer(_titleTapGesture);
            }

            if (_subtitleTapGesture == null &&
                ShowSubtitleContentCommand != null)
            {
                _subtitleTapGesture = new UITapGestureRecognizer(() =>
                    {
                        if (ShowSubtitleContentCommand != null &&
                            ShowSubtitleContentCommand.CanExecute(null))
                        {
                            ShowSubtitleContentCommand.Execute(null);
                        }
                    }) {
                        NumberOfTouchesRequired = (uint)1,
                        NumberOfTapsRequired = (uint)1
                    };

                SubtitleLabel.AddGestureRecognizer(_subtitleTapGesture);
            }
        }

        private void DisposeGestures()
        {
            if (_imageTapGesture != null)
            {
                BackgroundImage.RemoveGestureRecognizer(_imageTapGesture);
                _imageTapGesture.Dispose();
                _imageTapGesture = null;
            }

            if (_titleTapGesture != null)
            {
                TitleLabel.RemoveGestureRecognizer(_titleTapGesture);
                _titleTapGesture.Dispose();
                _titleTapGesture = null;
            }

            if (_subtitleTapGesture != null)
            {
                SubtitleLabel.RemoveGestureRecognizer(_subtitleTapGesture);
                _subtitleTapGesture.Dispose();
                _subtitleTapGesture = null;
            }
        }
    }
}