using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EntityCell : TableCellBase
    {
        private const int ImageHeight = 100;
        private const int TextLineHeight = 19;
        private const int Gap = 8;

        public static readonly UINib Nib = UINib.FromName("EntityCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EntityCell");

        private UITapGestureRecognizer _descriptionTapGesture;
        private CAGradientLayer _bottomGradient;

        public EntityCell(IntPtr handle) : base(handle)
        {
        }

        public static EntityCell Create()
        {
            return (EntityCell)Nib.Instantiate(null, null)[0];
        }

        public static float CalculateCellHeight(
            float frameWidth,
            bool isExpanded, 
            Entity entity)
        {
            var textHeight = CalculateTextHeight(frameWidth - Gap * 2, entity.Description);
            var result = 
                GetCollectionViewHeight(entity) + 
                ((int)textHeight != 0 ? Gap * 2 : 0) + 
                (isExpanded ? textHeight : Math.Min(textHeight, TextLineHeight * 3));
            return result;
        }

        private static float CalculateTextHeight(float frameWidth, string text)
        {
            if (text != null && text != string.Empty)
            {
                var frameSize = new SizeF(
                    frameWidth,
                    float.MaxValue); 

                SizeF textSize;
                using (var ns = new NSString(text))
                {
                    textSize = ns.StringSize(
                        UIFont.FromName("Helvetica", 15),
                        frameSize,
                        UILineBreakMode.TailTruncation);
                }

                return textSize.Height;
            }

            return 0;
        }

        private static float GetCollectionViewHeight(Entity entity)
        {
            if (entity.Info.HasLogo() || entity.Info.HasAddress())
            {
                return ScreenUtil.IsVerticalOrientation ? ImageHeight * 2 : ImageHeight;
            }

            return 0;
        }

        public ICommand ExpandCollapseCommand { get; set; }
        public ICommand ShowImageFullscreenCommand { get; set; }
        public ICommand NavigateWebSiteCommand { get; set; }
        public ICommand NavigateAddressesCommand { get; set; }

        public new IEntityCellContext DataContext
        {
            get { return (IEntityCellContext)base.DataContext; }
            set { base.DataContext = value; }
        }

        private EntityInfo DataContextEntityInfo
        {
            get { return DataContext != null ? DataContext.Entity.Info : null; }
        }

        private EntityCollectionSource CollectionSource
        {
            get { return (EntityCollectionSource)CollectionView.WeakDataSource; }
            set { CollectionView.WeakDataSource = value; }
        }

        private int CollectionSourceItemsCount
        {
            get
            {
                return CollectionSource != null && 
                        CollectionSource.ItemsSource != null
                    ? CollectionSource.ItemsSource.Cast<object>().Count() 
                    : 0;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            UpdateCollectionViewState();
            UpdateBottomGradientHiddenState();
            
            UpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            var collectionViewHeight = DataContext != null && DataContext.Entity != null 
                ? GetCollectionViewHeight(DataContext.Entity)
                : 0;
            if (Frame.Height >= collectionViewHeight)
            {
                CollectionViewHeightConstraint.Constant = collectionViewHeight;
            }
            else
            {
                CollectionViewHeightConstraint.Constant = ImageHeight;
            }

            if (DataContext != null &&
                DataContext.Entity.Description != null)
            {
                DescriptionTopConstraint.Constant = Gap;
                DescriptionBottomConstraint.Constant = Gap;
            }
            else
            {
                DescriptionTopConstraint.Constant = 0;
                DescriptionBottomConstraint.Constant = 0;
            }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                ExpandCollapseCommand = null;
                ShowImageFullscreenCommand = null;
                NavigateWebSiteCommand = null;
                NavigateAddressesCommand = null;

                DisposeGestures();
                DisposeCollectionView();
            }
        }

        protected override void OnInitialize()
        {
            InitializeGestures();
            InitializeCollectionView();
            InitializeBottomGradientState();

            SetNeedsLayout();
        }

        protected override void OnDataContextChanged(object previousContext, object newContext)
        {
            DescriptionLabel.Text = DataContext != null 
                ? DataContext.Entity.Description : null;

            var collectionItems = new List<object>();
            if (DataContext.Entity.Info.HasLogo())
            {
                collectionItems.Add(new ImageCollectionItem { EntityInfo = DataContext.Entity.Info });
            }

            if (DataContext.Entity.Info.HasAddress())
            {
                collectionItems.Add(new MapCollectionItem { Entity = DataContext.Entity });
            }

            CollectionSource.ItemsSource =
                DataContext != null 
                    ? collectionItems
                    : null;

            SetNeedsLayout();
            SetNeedsUpdateConstraints();
        }

        private void InitializeGestures()
        {
            _descriptionTapGesture = new UITapGestureRecognizer(() => {
                if (ExpandCollapseCommand != null &&
                    ExpandCollapseCommand.CanExecute(null))
                {
                    ExpandCollapseCommand.Execute(null);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            DescriptionLabel.AddGestureRecognizer(_descriptionTapGesture);
        }

        private void DisposeGestures()
        {
            if (_descriptionTapGesture != null)
            {
                DescriptionLabel.RemoveGestureRecognizer(_descriptionTapGesture);
                _descriptionTapGesture.Dispose();
                _descriptionTapGesture = null;
            }
        }

        private void InitializeCollectionView()
        {
            if (CollectionView.Source == null)
            {
                var collectionSource = new EntityCollectionSource(CollectionView) {
                    ShowImageFullscreenCommand = ShowImageFullscreenCommand,
                    NavigateWebSiteCommand = NavigateWebSiteCommand,
                    NavigateAddressesCommand = NavigateAddressesCommand
                };

                CollectionView.Source = collectionSource;
                CollectionView.ReloadData();
            }
        }

        private void DisposeCollectionView()
        {
            if (CollectionView != null &&
                CollectionView.WeakDataSource is EntityCollectionSource)
            {
                CollectionSource.ShowImageFullscreenCommand = null;
                CollectionSource.NavigateWebSiteCommand = null;
                CollectionSource.NavigateAddressesCommand = null;
            }
        }

        private void InitializeBottomGradientState()
        {
            _bottomGradient = new CAGradientLayer
                {
                    Frame = BottomGradientView.Bounds,
                    Colors = new [] { 
                        UIColor.White.ColorWithAlpha(0.2f).CGColor, 
                        UIColor.White.CGColor 
                    },
                };

            BottomGradientView.Layer.InsertSublayer(_bottomGradient, 0);
        }

        private void UpdateBottomGradientHiddenState()
        {
            if (_bottomGradient == null) return;

            // HACK: getting width from Cell's bounds, because Label's ones aren't updated yet
            var frame = _bottomGradient.Frame;
            frame.Width = Bounds.Width - Gap * 2;
            _bottomGradient.Frame = frame;

            if (DataContext != null && 
                !DataContext.IsDescriptionExpanded)
            {
                var textHeight = CalculateTextHeight(
                    DescriptionLabel.Frame.Width, 
                    DataContext.Entity.Description);

                var labelHeight = CalculateCellHeight(
                    Frame.Width, 
                    false, 
                    DataContext.Entity) - 
                        GetCollectionViewHeight(DataContext.Entity);

                _bottomGradient.Hidden = textHeight <= labelHeight;
            }
            else
            {
                _bottomGradient.Hidden = true;
            }
        }

        private void UpdateCollectionViewState()
        {
            if (DataContext != null)
            {
                CollectionView.CellHeight = ScreenUtil.IsVerticalOrientation &&
                    CollectionSourceItemsCount == 1
                        ? ImageHeight * 2
                        : ImageHeight;

                CollectionView.ItemsInRowCount = ScreenUtil.IsVerticalOrientation ||
                    CollectionSourceItemsCount == 2
                        ? null
                        : (int?)1;
            }
        }
    }
}