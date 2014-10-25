using System;
using System.Linq;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public partial class ContactsView : DialogViewBase
    {
        public static readonly UINib Nib = UINib.FromName("ContactsView", NSBundle.MainBundle);

        private const float Gap = 8;
        private const float DefaultPlaceholderMargin = 65;
        private const float DefaultPlaceholderLandscapeMargin = 55;

        private Entity _entity;
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
                if (!Equals(_entity, value))
                {
                    _entity = value;

                    Initialize();

                    ((ContactCollectionSource)CollectionView.WeakDataSource)
                        .ItemsSource =
                            _entity != null ? _entity.Contacts : null;

                    SetNeedsUpdateConstraints();
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
                NavigateWebSiteCommand = null;
            }
        }

        public override void LayoutSubviews()
        {
            UpdateConstraints();

            base.LayoutSubviews();
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

        protected override void OnInitialize()
        {
            base.OnInitialize();

            InitializeStyle();
            InitializeCollectionView();
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
            BackgroundView.BackgroundColor = Theme.DialogOutside;
            PlaceholderView.Layer.CornerRadius = 6;

            CloseButton.Layer.BorderWidth = 1;
            CloseButton.Layer.CornerRadius = 4;
            CloseButton.Layer.BorderColor = UIColor.Gray.CGColor;

            CloseButton.TouchDown += (sender, e) => CloseButton.BackgroundColor = UIColor.DarkGray;
            CloseButton.TouchUpInside += (sender, e) => CloseButton.BackgroundColor = Theme.CellBackground;
            CloseButton.TouchUpOutside += (sender, e) => CloseButton.BackgroundColor = Theme.CellBackground;
        }

        partial void OnCloseButtonClick(NSObject sender, UIEvent @event)
        {
            CloseView();
        }
    }
}