using System;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class ImageBackgroundView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ImageBackgroundView", NSBundle.MainBundle);

        private const int Gap = 10;
        private const int ButtonWidth = 40;
        private const int SubtitleHeight = 50;

        private readonly MvxImageViewLoader _imageHelper;
        private UITapGestureRecognizer _imageTapGesture;
        private UITapGestureRecognizer _subtitleTapGesture;
        private CAGradientLayer _bottomGradient;

        public ImageBackgroundView(IntPtr handle) : base(handle)
        {
            _imageHelper = new MvxImageViewLoader(
                () => BackgroundImage,
                () =>
                {
                    if (_imageHelper.ImageUrl != null &&
                        BackgroundImage.Image != null)
                    {
                        BackgroundImage.StopProgress();
                    }
                    else if (_imageHelper.ImageUrl == null)
                    {
                        BackgroundImage.StopProgress();
                    }
                    else
                    {
                        BackgroundImage.StartProgress();
                    }
                });
        }

        public static ImageBackgroundView Create()
        {
            return (ImageBackgroundView)Nib.Instantiate(null, null)[0];
        }

        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand ShowSubtitleContentCommand { get; set; }

        public string ImageUrl
        {
            get { return _imageHelper.ImageUrl; }
            set
            { 
                BackgroundImage.Image = null;
                _imageHelper.ImageUrl = value; 
            }
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

            if (Subtitle == null)
            {
                SubtitleLabel.Hidden = true;
                TitleBottomConstraint.Constant = Gap;
            }
            else
            {
                SubtitleLabel.Hidden = false;
                TitleBottomConstraint.Constant = SubtitleHeight;
            }

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
        }

        public void Initialize()
        {
            InitializeStyle();
            InitializeBottomGradientState();
            InitializeGestures();

            BackgroundImage.ActivityIndicatorViewStyle = 
                UIActivityIndicatorViewStyle.White;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

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

        private void InitializeStyle()
        {
            TitleLabel.Font = Theme.ImageTitleTextFont;
            TitleLabel.TextColor = Theme.ImageTitleText;
            TitleLabel.ShadowColor = Theme.ImageTextShadow;

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
                        Theme.ImageGradient.ColorWithAlpha(0.95f).CGColor 
                    },
                    Locations = new [] {
                        new NSNumber(0),
                        new NSNumber(0.2),
                        new NSNumber(1)
                    },
                };

                GradientPlaceholder.Layer.InsertSublayer(_bottomGradient, 0);
            }
        }

        private void InitializeGestures()
        {
            _imageTapGesture = new UITapGestureRecognizer(() => {
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

            _subtitleTapGesture = new UITapGestureRecognizer(() => {
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

        private void DisposeGestures()
        {
            if (_imageTapGesture != null)
            {
                BackgroundImage.RemoveGestureRecognizer(_imageTapGesture);
                _imageTapGesture.Dispose();
                _imageTapGesture = null;
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