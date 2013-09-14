using System;
using System.Linq;
using System.Collections;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using System.Drawing;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class ContactsView : UIView
    {
        public static readonly UINib Nib = UINib.FromName("ContactsView", NSBundle.MainBundle);

        private const float CellHeight = 40;
        private const float Gap = 8;
        private const float DefaultPlaceholderMargin = 40;

        private EntityInfo _entityInfo;
        private UITapGestureRecognizer _backgroundTapGesture;
        private ICommand _navigateWebSiteCommand;

        public ContactsView(IntPtr handle) : base(handle)
        {
        }

        public static ContactsView Create()
        {
            return (ContactsView)Nib.Instantiate(null, null)[0];
        }

        public event EventHandler Close;

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

        public EntityInfo EntityInfo
        {
            get
            {
                return _entityInfo;
            }
            set
            {
                if (!Equals(_entityInfo, value))
                {
                    _entityInfo = value;

                    InitializeGestures();
                    InitializeCollectionView();

                    ((ContactCollectionSource)CollectionView.WeakDataSource).ItemsSource =
                        _entityInfo != null 
                            ? (IEnumerable)new ContactCollectionSourceConverter()
                            .Convert(_entityInfo.Contact, typeof(IEnumerable), null, null) 
                            : null;

                    SetNeedsUpdateConstraints();
                }
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                NavigateWebSiteCommand = null;

                DisposeGestures();
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
                var collectionHeight = itemsCount * CellHeight + (itemsCount - 1) * Gap;
                var placeholderHeight = 
                    CollectionTopConstraint.Constant +
                        collectionHeight +
                        CollectionBottomConstraint.Constant;
                var placeholderMargin = (Bounds.Height - placeholderHeight) / 2.0;

                PlaceholderTopConstraint.Constant = Math.Max((float)placeholderMargin, DefaultPlaceholderMargin);
                PlaceholderBottomConstraint.Constant = Math.Max((float)placeholderMargin, DefaultPlaceholderMargin);
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
                    NavigateSiteLinkCommand = NavigateWebSiteCommand
                };
            }
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