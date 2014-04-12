using System;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class ContactsView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ContactsView", NSBundle.MainBundle);

        private const float Gap = 8;
        private const float DefaultPlaceholderMargin = 40;
        private const float DefaultPlaceholderLandscapeMargin = 15;

        private Entity _entity;
        private UITapGestureRecognizer _backgroundTapGesture;
        private ICommand _callPhoneCommand;
        private ICommand _composeEmailCommand;
        private ICommand _navigateWebSiteCommand;

        public ContactsView(IntPtr handle) : base(handle)
        {
        }

        public static ContactsView Create()
        {
            return (ContactsView)Nib.Instantiate(null, null)[0];
        }

        public event EventHandler Close;

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

        public ICommand NavigateWebSiteCommand
        {
            get
            {
                return _navigateWebSiteCommand;
            }
            set
            {
                _navigateWebSiteCommand = value;

                var collectionDelegate = CollectionView.WeakDelegate as ContactCollectionDelegate;
                if (collectionDelegate != null)
                {
                    collectionDelegate.NavigateSiteLinkCommand = _navigateWebSiteCommand;
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
                var isInitialized = _entity != null;

                if (!Equals(_entity, value))
                {
                    _entity = value;

                    if (!isInitialized)
                    {
                        InitializeStyle();
                        InitializeGestures();
                        InitializeCollectionView();
                    }

                    ((ContactCollectionSource)CollectionView.WeakDataSource).ItemsSource =
                        _entity != null ? _entity.Contacts : null;

                    SetNeedsUpdateConstraints();
                }
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                CallPhoneCommand = null;
                ComposeEmailCommand = null;
                NavigateWebSiteCommand = null;

                DisposeGestures();
            }
        }

        public override void LayoutSubviews()
        {
            UpdateConstraints();

            base.LayoutSubviews();

            ((UICollectionViewFlowLayout)CollectionView.CollectionViewLayout).ItemSize = 
                new SizeF(ScreenUtil.IsVerticalOrientation ? 304 : 400, ContactCell.DefaultHeight);
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            var colletionSource = CollectionView.WeakDataSource as ContactCollectionSource;
            if (colletionSource != null)
            {
                var itemsCount = colletionSource.ItemsSource != null
                    ? colletionSource.ItemsSource.Cast<object>().Count()
                        : 0;
                var collectionHeight = itemsCount * ContactCell.DefaultHeight + (itemsCount - 1) * Gap;
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
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
                    NavigateSiteLinkCommand = NavigateWebSiteCommand
                };
            }
        }

        private void InitializeStyle()
        {
            CloseButton.Layer.BorderWidth = 1;
            CloseButton.Layer.CornerRadius = 4;
            CloseButton.Layer.BorderColor = UIColor.Gray.CGColor;

            CloseButton.TouchDown += (sender, e) => CloseButton.BackgroundColor = UIColor.DarkGray;
            CloseButton.TouchUpInside += (sender, e) => CloseButton.BackgroundColor = Theme.CellBackground;
            CloseButton.TouchUpOutside += (sender, e) => CloseButton.BackgroundColor = Theme.CellBackground;
        }

        private void InitializeGestures()
        {
            if (_backgroundTapGesture == null)
            {
                _backgroundTapGesture = new UITapGestureRecognizer(CloseView) {
                    NumberOfTouchesRequired = (uint)1,
                    NumberOfTapsRequired = (uint)1
                };

                BackgroundView.AddGestureRecognizer(_backgroundTapGesture);
            }
        }

        private void DisposeGestures()
        {
            if (_backgroundTapGesture != null)
            {
                BackgroundView.RemoveGestureRecognizer(_backgroundTapGesture);
                _backgroundTapGesture.Dispose();
                _backgroundTapGesture = null;
            }
        }

        partial void OnCloseButtonClick(NSObject sender, UIEvent @event)
        {
            CloseView();
        }

        private void CloseView()
        {
            if (Close != null)
            {
                Close(this, EventArgs.Empty);
            }
        }
    }
}