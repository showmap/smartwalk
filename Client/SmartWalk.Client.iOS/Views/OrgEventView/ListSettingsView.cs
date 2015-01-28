using System;
using CoreGraphics;
using System.Windows.Input;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class ListSettingsView : DialogViewBase
    {
        public static readonly UINib Nib = UINib.FromName("ListSettingsView", NSBundle.MainBundle);

        private const int DefaultHeight = 88;
        private const int CollapsedHeight = 44;

        public ListSettingsView(IntPtr handle) : base(handle)
        {
        }

        public static ListSettingsView Create()
        {
            return (ListSettingsView)Nib.Instantiate(null, null)[0];
        }

        public ICommand GroupByLocationCommand { get; set; }
        public ICommand SortByCommand { get; set; }

        public nfloat MarginTop
        {
            get { return ContainerTopConstraint.Constant; }
            set { ContainerTopConstraint.Constant = value; }
        }

        public bool IsGroupByLocation
        {
            get { return GroupByLocationSwitch.On; }
            set { GroupByLocationSwitch.On = value; }
        }

        public SortBy SortBy
        {
            get { return SortBySegments.SelectedSegment == 0 ? SortBy.Time : SortBy.Name; }
            set { SortBySegments.SelectedSegment = (int)value; }
        }

        protected override UIView OutsideAreaView
        {
            get { return BackgroundView; }
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                GroupByLocationCommand = null;
                SortByCommand = null;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            InitializeStyle();
            UpdateViewState(false);
        }

        partial void OnGroupByLocationTouchUpInside(NSObject sender)
        {
            if (GroupByLocationCommand != null &&
                GroupByLocationCommand.CanExecute(IsGroupByLocation))
            {
                GroupByLocationCommand.Execute(IsGroupByLocation);
            }

            UpdateViewState(true);
        }

        partial void OnSortBySegmentsValueChanged(NSObject sender)
        {
            if (SortByCommand != null &&
                SortByCommand.CanExecute(SortBy))
            {
                SortByCommand.Execute(SortBy);
            }
        }

        private void InitializeStyle()
        {
            TopSeparator.IsLineOnTop = true;

            PlaceholderView.BackgroundColor = ThemeColors.PanelBackgroundAlpha;
            PlaceholderView.Layer.ShadowColor = UIColor.Black.CGColor;
            PlaceholderView.Layer.ShadowOffset = new CGSize(0, 5);
            PlaceholderView.Layer.ShadowOpacity = 0.3f;

            GroupByLocationLabel.Font = Theme.OrgEventHeaderFont;
            GroupByLocationLabel.TextColor = ThemeColors.ContentLightText;

            SortByLabel.Font = Theme.OrgEventHeaderFont;
            SortByLabel.TextColor = ThemeColors.ContentLightText;
        }

        private void UpdateViewState(bool animated)
        {
            if (!animated)
            {
                ContainerHeightConstraint.Constant = 
                    IsGroupByLocation ? CollapsedHeight : DefaultHeight;
                SortByPlaceholder.Hidden = IsGroupByLocation;
                return;
            }

            if (IsGroupByLocation)
            {
                UIView.Transition(
                    SortByPlaceholder, 
                    UIConstants.AnimationDuration, 
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    () => {},
                    () => 
                    {
                        LayoutIfNeeded();
                        UIView.Animate(
                            UIConstants.AnimationDuration, 
                            () =>
                            {
                                ContainerHeightConstraint.Constant = CollapsedHeight;
                                LayoutIfNeeded();
                            });
                    });

                SortByPlaceholder.Hidden = true;
            }
            else
            {
                LayoutIfNeeded();
                UIView.Animate(
                    UIConstants.AnimationDuration, 
                    () =>
                    {
                        ContainerHeightConstraint.Constant = DefaultHeight;
                        LayoutIfNeeded();
                    },
                    () =>
                    {
                        UIView.Transition(
                            SortByPlaceholder, 
                            UIConstants.AnimationDuration, 
                            UIViewAnimationOptions.TransitionCrossDissolve,
                            () => {},
                            null);

                        SortByPlaceholder.Hidden = false;
                    });
            }
        }
    }
}