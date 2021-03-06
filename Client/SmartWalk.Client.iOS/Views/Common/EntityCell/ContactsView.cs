using System;
using System.Linq;
using System.Windows.Input;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class ContactsView : DialogViewBase
    {
        public static readonly UINib Nib = UINib.FromName("ContactsView", NSBundle.MainBundle);

        private const float DefaultPlaceholderMargin = 65;
        private const float DefaultPlaceholderLandscapeMargin = 55;

        private Entity _entity;
        private ICommand _callPhoneCommand;
        private ICommand _composeEmailCommand;
        private ICommand _showHideModalViewCommand;

        private NSObject _orientationObserver;
        private bool _updateScheduled;

        public ContactsView(IntPtr handle) : base(handle)
        {
        }

        public static ContactsView Create()
        {
            return (ContactsView)Nib.Instantiate(null, null)[0];
        }

        public ICommand CallPhoneCommand
        {
            get
            {
                return _callPhoneCommand;
            }
            set
            {
                _callPhoneCommand = value;

                var collectionDelegate = CollectionView.WeakDelegate as ContactCollectionDelegate;
                if (collectionDelegate != null)
                {
                    collectionDelegate.CallPhoneCommand = _callPhoneCommand;
                }
            }
        }

        public ICommand ComposeEmailCommand
        {
            get
            {
                return _composeEmailCommand;
            }
            set
            {
                _composeEmailCommand = value;

                var collectionDelegate = CollectionView.WeakDelegate as ContactCollectionDelegate;
                if (collectionDelegate != null)
                {
                    collectionDelegate.ComposeEmailCommand = _composeEmailCommand;
                }
            }
        }

        public ICommand ShowHideModalViewCommand
        {
            get
            {
                return _showHideModalViewCommand;
            }
            set
            {
                _showHideModalViewCommand = value;

                var collectionDelegate = CollectionView.WeakDelegate as ContactCollectionDelegate;
                if (collectionDelegate != null)
                {
                    collectionDelegate.ShowHideModalViewCommand = _showHideModalViewCommand;
                }
            }
        }

        public Entity Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                if (!Equals(_entity, value))
                {
                    _entity = value;

                    Initialize();

                    ((ContactCollectionSource)CollectionView.WeakDataSource)
                        .ItemsSource =
                            _entity != null ? _entity.Contacts : null;

                    UpdateConstraintConstants();
                }
            }
        }

        protected override UIView OutsideAreaView
        {
            get
            {
                return BackgroundView;
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                CallPhoneCommand = null;
                ComposeEmailCommand = null;
                ShowHideModalViewCommand = null;

                DisposeOrientationObserver();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_updateScheduled)
            {
                UpdateConstraintConstants();
                _updateScheduled = false;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            InitializeStyle();
            InitializeCollectionView();
            InitializeOrientationObserver();
        }

        private void InitializeCollectionView()
        {
            if (CollectionView.Source == null)
            {
                var collectionSource = new ContactCollectionSource(CollectionView);
                CollectionView.Source = collectionSource;

                CollectionView.Delegate = new ContactCollectionDelegate(collectionSource) {
                    CallPhoneCommand = CallPhoneCommand,
                    ComposeEmailCommand = ComposeEmailCommand,
                    ShowHideModalViewCommand = ShowHideModalViewCommand
                };
            }
        }

        private void InitializeStyle()
        {
            BackgroundView.BackgroundColor = ThemeColors.ContentDarkBackground.ColorWithAlpha(200);
            PlaceholderView.Layer.CornerRadius = 6;

            CloseButton.Layer.BorderWidth = 1;
            CloseButton.Layer.CornerRadius = 4;
            CloseButton.Layer.BorderColor = ThemeColors.BorderLight.CGColor;

            CloseButton.TouchDown += (sender, e) => CloseButton.BackgroundColor = ThemeColors.ContentLightHighlight;
            CloseButton.TouchUpInside += (sender, e) => CloseButton.BackgroundColor = ThemeColors.ContentLightBackground;
            CloseButton.TouchUpOutside += (sender, e) => CloseButton.BackgroundColor = ThemeColors.ContentLightBackground;
        }

        private void InitializeOrientationObserver()
        {
            _orientationObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                UIDevice.OrientationDidChangeNotification,
                OnDeviceOrientationDidChange);

            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
        }

        private void OnDeviceOrientationDidChange(NSNotification notification)
        {
            if (Window == null)
            {
                _updateScheduled = true;
                return;
            }

            UpdateConstraintConstants();
        }

        private void DisposeOrientationObserver()
        {
            if (_orientationObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_orientationObserver);
            }
        }

        private void UpdateConstraintConstants()
        {
            var colletionSource = CollectionView.WeakDataSource as ContactCollectionSource;
            if (colletionSource != null)
            {
                var itemsCount = colletionSource.ItemsSource != null
                    ? colletionSource.ItemsSource.Cast<object>().Count()
                    : 0;
                var collectionHeight = itemsCount * ContactCell.DefaultHeight;
                var placeholderHeight = 
                    CollectionTopConstraint.Constant +
                    collectionHeight +
                    CollectionBottomConstraint.Constant;
                var placeholderMargin = (Bounds.Height - placeholderHeight) / 2.0;

                var defaultMargin = ScreenUtil.IsVerticalOrientation 
                    ? DefaultPlaceholderMargin 
                    : DefaultPlaceholderLandscapeMargin; 

                PlaceholderTopConstraint.Constant = Math.Max((float)placeholderMargin, defaultMargin);
                PlaceholderBottomConstraint.Constant = Math.Max((float)placeholderMargin, defaultMargin);
            }
        }

        partial void OnCloseButtonClick(NSObject sender, UIEvent @event)
        {
            CloseView();
        }
    }
}