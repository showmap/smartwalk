using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.CoreLocation;
using MonoTouch.EventKit;
using MonoTouch.EventKitUI;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.OrgEventView;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private OrgEventHeaderView _headerView;
        private UIBarButtonItem _modeButtonItem;
        private UIBarButtonItem _dayButtonItem;
        private UIBarButtonItem _customModeButtonItem;
        private UIBarButtonItem _customDayButtonItem;
        private UISearchDisplayController _searchDisplayController;
        private EKEventEditViewController _editCalEventController;
        private ListSettingsView _listSettingsView;
        private bool _isMapViewInitialized;
        private PointF _tableContentOffset;
        private Show _previousExpandedShow;
        private ButtonBarButton _modeListButton;
        private ButtonBarButton _modeMapButton;
        private ButtonBarButton _moreButton;
        private ButtonBarButton _dayButton;
        private ButtonBarButton _customDayButton;
        private NSLayoutConstraint[] _topLayoutGuideConstraint;

        private NSTimer _timer;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.ZoomSelectedVenue += OnZoomSelectedVenue;
            ViewModel.ScrollSelectedVenue += OnScrollSelectedVenue;

            InitializeToolBar();
            InitializeStyle();
            InitializeGestures();

            UpdateViewState(false);
            UpdateDayButtonState();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetStatusBarHidden(false, animated);
            UpdateNavBarState(animated);

            // TODO: Find another soltuion. It must be much simpler.
            // HACK: To persist table scroll offset on rotation and appearing
            if (_tableContentOffset != PointF.Empty && _timer == null)
            {
                _timer = NSTimer.CreateRepeatingScheduledTimer(
                    TimeSpan.MinValue, 
                    new NSAction(() => 
                    {
                        if (VenuesAndShowsTableView.TableHeaderView != null &&
                            VenuesAndShowsTableView.ContentSize.Height > 
                            VenuesAndShowsTableView.TableHeaderView.Frame.Height)
                        {
                            VenuesAndShowsTableView.SetContentOffset(_tableContentOffset, false);
                            _tableContentOffset = PointF.Empty;
                            _timer.Invalidate();
                            _timer.Dispose();
                            _timer = null;
                        }
                    }));
            }

            if (_listSettingsView != null)
            {
                SetDialogViewFullscreenFrame(_listSettingsView);
            }

            ButtonBarUtil.UpdateButtonsFrameOnRotation(
                new [] {_modeListButton, _modeMapButton, MapFullscreenButton});
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            #region HACK
            // HACK: To persist table scroll offset
            _tableContentOffset = VenuesAndShowsTableView.ContentOffset;

            // HACK: To hide nav bar after iOS make it visible in next view
            if (_searchDisplayController != null &&
                _searchDisplayController.Active)
            {
                if (NavBarManager.Instance.NativeHidden !=
                    NavigationController.NavigationBarHidden)
                {
                    NavigationController
                        .SetNavigationBarHidden(NavBarManager.Instance.NativeHidden, false);
                }
                else
                {
                    UIApplication.SharedApplication.KeyWindow
                        .RootViewController.View.LayoutSubviews();
                }
            }
            #endregion
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel.ZoomSelectedVenue -= OnZoomSelectedVenue;
                ViewModel.ScrollSelectedVenue -= OnScrollSelectedVenue;

                DisposeToolBar();
                DisposeGestures();
                DisposeTableHeader();
                DisposeSearchDisplayController();
                DisposeMapView();
            }
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            if (_listSettingsView != null)
            {
                SetDialogViewFullscreenFrame(_listSettingsView);
            }

            UpdateViewConstraints();
            ButtonBarUtil.UpdateButtonsFrameOnRotation(
                new [] {_modeListButton, _modeMapButton, MapFullscreenButton});

            // HACK: hiding jerking search bar on rotation
            if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                if (_headerView != null)
                {
                    _headerView.SearchBarControl.Hidden = true;
                }
            }
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            // HACK: showing jerking search bar on rotation
            if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                if (_headerView != null)
                {
                    _headerView.SearchBarControl.Hidden = false;
                }
            }
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();

            UpdateViewConstraints(false);
        }

        protected override void UpdateStatusBarLoadingState(bool animated)
        {
            // overriding the base class logic of showing status bar during loading
            // status bar is always visible in this view
        }

        private void UpdateViewConstraints(bool animated)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0) &&
                _topLayoutGuideConstraint == null)
            {
                _topLayoutGuideConstraint = 
                    NSLayoutConstraint.FromVisualFormat(
                        "V:[topGuide]-0-[view]", 
                        0, 
                        null, 
                        new NSDictionary(
                            "topGuide", 
                            TopLayoutGuide, 
                            "view", 
                            TablePanel));
            }

            switch (ViewModel.Mode)
            {
                case OrgEventViewMode.Combo:
                    if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                    {
                        EdgesForExtendedLayout = UIRectEdge.None;
                        View.RemoveConstraints(_topLayoutGuideConstraint);

                        if (!View.Constraints.Contains(MapToTableConstraint))
                        {
                            View.AddConstraint(MapToTableConstraint);
                        }
                    }

                    TablePanel.RemoveConstraint(TableHeightConstraint);

                    if (!MapPanel.Constraints.Contains(MapHeightConstraint))
                    {
                        MapPanel.AddConstraint(MapHeightConstraint);
                    }

                    UpdateConstraint(
                        () => MapHeightConstraint.Constant = ScreenUtil.GetGoldenRatio(View.Frame.Height),
                        animated);
                    break;

                case OrgEventViewMode.Map:
                    if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                    {
                        EdgesForExtendedLayout = UIRectEdge.None;
                        View.RemoveConstraints(_topLayoutGuideConstraint);

                        if (!View.Constraints.Contains(MapToTableConstraint))
                        {
                            View.AddConstraint(MapToTableConstraint);
                        }
                    }

                    MapPanel.RemoveConstraint(MapHeightConstraint);

                    if (!TablePanel.Constraints.Contains(TableHeightConstraint))
                    {
                        TablePanel.AddConstraint(TableHeightConstraint);
                    }

                    UpdateConstraint(
                        () => TableHeightConstraint.Constant = 0,
                        animated);
                    break;

                case OrgEventViewMode.List:
                    TablePanel.RemoveConstraint(TableHeightConstraint);

                    if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                    {
                        EdgesForExtendedLayout = UIRectEdge.Top;
                        View.RemoveConstraint(MapToTableConstraint);

                        if (!_topLayoutGuideConstraint.Any(c => View.Constraints.Contains(c)))
                        {
                            View.AddConstraints(_topLayoutGuideConstraint);
                        }
                    }

                    if (!MapPanel.Constraints.Contains(MapHeightConstraint))
                    {
                        MapPanel.AddConstraint(MapHeightConstraint);
                    }

                    UpdateConstraint(
                        () => MapHeightConstraint.Constant = 0,
                        animated);
                    break;
            }

            var size = ScreenUtil.IsVerticalOrientation 
                ? ButtonBarButton.DefaultVerticalSize
                : ButtonBarButton.DefaultLandscapeSize;
            FullscreenWidthConstraint.Constant = size.Width;
            FullscreenHeightConstraint.Constant = size.Height;
        }

        private void UpdateConstraint(Action updateHandler, bool animated)
        {
            const double animationSpeed = 0.2;

            if (animated)
            {
                UIView.Animate(
                    animationSpeed, 
                    () =>
                    {
                        updateHandler();
                        View.LayoutIfNeeded();
                    });
            }
            else
            {
                updateHandler();
            }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenuesAndShowsTableView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override NSLayoutConstraint GetProgressViewTopConstraint()
        {
            return ProgressViewTopConstraint;
        }

        protected override IListViewSource CreateListViewSource()
        {
            var tableSource = new OrgEventTableSource(ViewModel)
                {
                    TableView = VenuesAndShowsTableView
                };

            this.CreateBinding(tableSource)
                .For(ts => ts.ItemsSource)
                .To<OrgEventViewModel>(vm => vm.ListItems)
                .WithConversion(new OrgEventTableSourceConverter(), ViewModel)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewState(true);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.CurrentMapType))
            {
                VenuesMapView.MapType = ViewModel.CurrentMapType.ToMKMapType();
                ReloadMap();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.OrgEvent))
            {
                ReloadMap();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                if (ViewModel.Mode == OrgEventViewMode.Map ||
                    ViewModel.Mode == OrgEventViewMode.Combo ||
                    ViewModel.SelectedVenueOnMap == null)
                {
                    SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                UpdateTableViewOnShowExpanding(VenuesAndShowsTableView);

                if (SearchDisplayController.SearchResultsTableView.Superview != null &&
                    !SearchDisplayController.SearchResultsTableView.Hidden)
                {
                    UpdateTableViewOnShowExpanding(SearchDisplayController.SearchResultsTableView);
                }

                _previousExpandedShow = ViewModel.ExpandedShow;
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.CurrentCalendarEvent))
            {
                if (ViewModel.CurrentCalendarEvent != null)
                {
                    if (_editCalEventController == null)
                    {
                        InitializeCalEventViewController();
                    }
                }
                else
                {
                    DisposeCalEventViewController();
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsListOptionsAvailable))
            {
                UpdateTableHeaderState();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsListOptionsShown))
            {
                ShowHideListSettingsView(ViewModel.IsListOptionsShown);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsMultiday) ||
                propertyName == ViewModel.GetPropertyName(vm => vm.CurrentDayTitle))
            {
                UpdateDayButtonState();
            }
        }

        protected override void OnViewModelRefreshed(bool hasData)
        {
            base.OnViewModelRefreshed(hasData);

            if (ViewModel.OrgEvent == null)
            {
                MapContentView.Hidden = true;
            }
        }

        protected override void ScrollViewToTop()
        {
            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource<Venue>;
            if (tableSource != null)
            {
                tableSource.ScrollOutHeader();
            }
            else
            {
                base.ScrollViewToTop();
            }
        }

        // set header before items source is passed to table
        // for easier header auto-hiding
        protected override void OnBeforeSetListViewSource()
        {   
            InitializeTableHeader();
            InitializeSearchDisplayController();
        }

        protected override void OnLoadingViewStateUpdate()
        {
            base.OnLoadingViewStateUpdate();

            if (ViewModel.OrgEvent == null)
            {
                MapContentView.Hidden = true;
            }
        }

        protected override void OnLoadedViewStateUpdate()
        {
            MapContentView.SetHidden(false, true);

            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource<Venue>;
            if (tableSource == null || tableSource.IsHeaderViewHidden)
            {
                base.OnLoadedViewStateUpdate();
            }
        }

        protected override void OnInitializingActionSheet(UIActionSheet actionSheet)
        {
            if (ViewModel.CreateEventCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.SaveToCalendar);
            }

            if (ViewModel.NavigateOrgEventInfoCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.ShowEventInfo);
            }

            if (ViewModel.NavigateOrgCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.ShowOrganizerInfo);
            }

            if (ViewModel.SwitchMapTypeCommand.CanExecute(null))
            {
                actionSheet.AddButton(ViewModel.CurrentMapType.GetMapTypeButtonLabel());
            }

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.SaveToCalendar:
                    if (ViewModel.CreateEventCommand.CanExecute(null))
                    {
                        ViewModel.CreateEventCommand.Execute(null);
                    }
                    break;

                case Localization.ShowEventInfo:
                    if (ViewModel.NavigateOrgEventInfoCommand.CanExecute(null))
                    {
                        ViewModel.NavigateOrgEventInfoCommand.Execute(null);
                    }
                    break;

                case Localization.ShowOrganizerInfo:
                    if (ViewModel.NavigateOrgCommand.CanExecute(null))
                    {
                        ViewModel.NavigateOrgCommand.Execute(null);
                    }
                    break;

                case Localization.CopyLink:
                    if (ViewModel.CopyLinkCommand.CanExecute(null))
                    {
                        ViewModel.CopyLinkCommand.Execute(null);
                    }
                    break;

                case Localization.ShareButton:
                    if (ViewModel.ShareCommand.CanExecute(null))
                    {
                        ViewModel.ShareCommand.Execute(null);
                    }
                    break;
            }

            if (buttonTitle == ViewModel.CurrentMapType.GetMapTypeButtonLabel())
            {
                if (ViewModel.SwitchMapTypeCommand.CanExecute(null))
                {
                    ViewModel.SwitchMapTypeCommand.Execute(null);
                }
            }
        }

        protected override void OnInitializeCustomNavBarItems(List<UIBarButtonItem> navBarItems)
        {
            base.OnInitializeCustomNavBarItems(navBarItems);

            _customDayButton = ButtonBarUtil.Create(true);
            _customDayButton.TouchUpInside += OnDayButtonClicked;

            _customDayButtonItem = new UIBarButtonItem();
            _customDayButtonItem.CustomView = _customDayButton;

            _customModeButtonItem = new UIBarButtonItem();

            navBarItems.Add(_customDayButtonItem);
            navBarItems.Add(_customModeButtonItem);
        }

        private void InitializeStyle()
        {
            MapFullscreenButton.IsSemiTransparent = true;

            _customDayButton.Font = Theme.NavBarFont;
            _customDayButton.SetTitleColor(Theme.NavBarText, UIControlState.Normal);

            _dayButton.Font = Theme.NavBarFont;
            _dayButton.SetTitleColor(Theme.NavBarText, UIControlState.Normal);

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                VenuesMapView.TintColor = Theme.MapTint;
            }
        }

        private void InitializeToolBar()
        {
            _modeListButton = ButtonBarUtil.Create(ThemeIcons.NavBarList, ThemeIcons.NavBarListLandscape, true);
            _modeListButton.TouchUpInside += OnModeButtonClicked;

            _modeMapButton = ButtonBarUtil.Create(ThemeIcons.NavBarMap, ThemeIcons.NavBarMapLandscape);
            _modeMapButton.TouchUpInside += OnModeButtonClicked;

            _dayButton = ButtonBarUtil.Create();
            _dayButton.TouchUpInside += OnDayButtonClicked;
            _dayButtonItem = new UIBarButtonItem();
            _dayButtonItem.CustomView = _dayButton;

            _modeButtonItem = new UIBarButtonItem();
            var gap = ButtonBarUtil.CreateGapSpacer();

            _moreButton = ButtonBarUtil.Create(ThemeIcons.NavBarMore, ThemeIcons.NavBarMoreLandscape);
            _moreButton.TouchUpInside += OnMoreButtonClicked;

            var moreBarButton = new UIBarButtonItem(_moreButton);
            NavigationItem.SetRightBarButtonItems(new [] {gap, moreBarButton, _modeButtonItem, _dayButtonItem}, true);
        }

        private void DisposeToolBar()
        {
            if (_modeListButton != null)
            {
                _modeListButton.TouchUpInside -= OnModeButtonClicked;
            }

            if (_modeMapButton != null)
            {
                _modeMapButton.TouchUpInside -= OnModeButtonClicked;
            }

            if (_moreButton != null)
            {
                _moreButton.TouchUpInside -= OnMoreButtonClicked;
            }

            if (_dayButton != null)
            {
                _dayButton.TouchUpInside -= OnDayButtonClicked;
            }

            if (_customDayButton != null)
            {
                _customDayButton.TouchUpInside -= OnDayButtonClicked;
            }
        }

        private void OnModeButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.SwitchModeCommand.CanExecute(null))
            {
                ViewModel.SwitchModeCommand.Execute(null);
            }
        }

        private void OnDayButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.SetCurrentDayCommand.CanExecute(null))
            {
                ViewModel.SetCurrentDayCommand.Execute(null);
            }
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            ShowActionSheet();
        }

        partial void OnMapFullscreenTouchUpInside(NSObject sender)
        {
            if (ViewModel.SwitchMapFullscreenCommand.CanExecute(null))
            {
                ViewModel.SwitchMapFullscreenCommand.Execute(null);
            }
        }

        private void InitializeTableHeader()
        {
            _headerView = OrgEventHeaderView.Create();
            _headerView.ShowOptionsCommand = ViewModel.ShowHideListOptionsCommand;

            UpdateTableHeaderState();

            VenuesAndShowsTableView.TableHeaderView = _headerView;
        }

        private void DisposeTableHeader()
        {
            if (_headerView != null)
            {
                _headerView.Dispose();
                _headerView = null;
            }
        }

        private void UpdateTableHeaderState()
        {
            if (_headerView != null)
            {
                _headerView.IsListOptionsVisible = ViewModel.IsListOptionsAvailable;
            }
        }

        private void InitializeSearchDisplayController()
        {
            _searchDisplayController = 
                new ExtendedSearchDisplayController(_headerView.SearchBarControl, this);
            _searchDisplayController.Delegate = 
                new OrgEventSearchDelegate(_headerView, ViewModel);

            var searchTableSource = new OrgEventTableSource(ViewModel)
                {
                    IsSearchSource = true,
                    TableView = _searchDisplayController.SearchResultsTableView
                };

            this.CreateBinding(searchTableSource)
                .For(ts => ts.ItemsSource)
                .To<OrgEventViewModel>(vm => vm.SearchResults)
                .WithConversion(new OrgEventTableSourceConverter(), ViewModel)
                .Apply();

            _searchDisplayController.SearchResultsSource = searchTableSource;
        }

        private void DeactivateSearchController()
        {
            if (_searchDisplayController != null &&
                _searchDisplayController.Active)
            {
                _searchDisplayController.SetActive(false, false);
            }
        }

        // To avoid iOS exception sometimes
        // http://stackoverflow.com/questions/2758575/how-can-uisearchdisplaycontroller-autorelease-cause-crash-in-a-different-view-co
        private void DisposeSearchDisplayController()
        {
            if (_searchDisplayController != null)
            {
                _searchDisplayController.Delegate = null;
                _searchDisplayController.SearchResultsDelegate = null;
                _searchDisplayController.SearchResultsDataSource = null;
                _searchDisplayController.Dispose();
                _searchDisplayController = null;
            }
        }

        private void InitializeGestures()
        {
        }

        private void DisposeGestures()
        {
        }

        private void ReloadMap()
        {
            _isMapViewInitialized = false;

            if (ViewModel.Mode == OrgEventViewMode.Map ||
                ViewModel.Mode == OrgEventViewMode.Combo)
            {
                InitializeMapView();
            }
        }

        private void InitializeMapView()
        {
            if (ViewModel.OrgEvent != null &&
                ViewModel.OrgEvent.Venues != null)
            {
                if (!_isMapViewInitialized)
                {
                    VenuesMapView.RemoveAnnotations(VenuesMapView.Annotations);

                    if (!(VenuesMapView.WeakDelegate is MapDelegate))
                    {
                        var mapDelegate = new MapDelegate
                            {
                                SelectAnnotationCommand = ViewModel.SelectVenueOnMapCommand,
                                ShowDetailsCommand = ViewModel.NavigateVenueCommand
                            };
                        VenuesMapView.Delegate = mapDelegate;
                    }

                    var annotations = ViewModel.OrgEvent.Venues
                        .SelectMany(v => v.Info.Addresses
                            .Select(a => new VenueAnnotation(v, a))).ToArray();
                    var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);

                    VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), false);
                    VenuesMapView.AddAnnotations(annotations);

                    if (ViewModel.SelectedVenueOnMap != null)
                    {
                        SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                    }

                    _isMapViewInitialized = true;
                }
            }
            else
            {
                VenuesMapView.RemoveAnnotations(VenuesMapView.Annotations);
                _isMapViewInitialized = false;
            }
        }

        private void DisposeMapView()
        {
            VenuesMapView.RemoveAnnotations(VenuesMapView.Annotations);

            var mapDelegate = VenuesMapView.WeakDelegate as MapDelegate;
            if (mapDelegate != null)
            {
                mapDelegate.SelectAnnotationCommand = null;
                mapDelegate.ShowDetailsCommand = null;
                mapDelegate.Dispose();
                VenuesMapView.WeakDelegate = null;
            }
        }

        private void SelectVenueMapAnnotation(Venue venue)
        {
            if (venue != null)
            {
                var annotation = GetAnnotationByVenue(venue);
                if (annotation != null)
                {
                    VenuesMapView.SelectAnnotation(annotation, true);
                }
            }
            else if (VenuesMapView.SelectedAnnotations != null &&
                VenuesMapView.SelectedAnnotations.Any())
            {
                foreach (var annotation in VenuesMapView.SelectedAnnotations)
                {
                    VenuesMapView.DeselectAnnotation(annotation, false);
                }

                var annotations = VenuesMapView.Annotations.OfType<VenueAnnotation>();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);
                VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), true);
            }
        }

        private VenueAnnotation GetAnnotationByVenue(Venue venue)
        {
            var annotation = 
                VenuesMapView.Annotations
                    .OfType<VenueAnnotation>()
                    .FirstOrDefault(an => 
                        an.Venue.Info.Id == venue.Info.Id);
            return annotation;
        }

        private void OnZoomSelectedVenue(object sender, EventArgs e)
        {
            if (ViewModel.SelectedVenueOnMap != null)
            {
                var annotation = GetAnnotationByVenue(ViewModel.SelectedVenueOnMap);

                var shiftedCoord = annotation.Coordinate;
                shiftedCoord.Latitude += 0.0008f;

                VenuesMapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(shiftedCoord), true);
            }
        }

        private void OnScrollSelectedVenue(object sender, EventArgs e)
        {
            var tableSource = VenuesAndShowsTableView.WeakDelegate as OrgEventTableSource;
            if (tableSource != null &&
                ViewModel.SelectedVenueOnMap != null)
            {
                var index = tableSource.GetItemIndex(ViewModel.SelectedVenueOnMap);
                if (index != null)
                {
                    VenuesAndShowsTableView.ScrollToRow(
                        index, 
                        UITableViewScrollPosition.Top,
                        true);
                }
            }
        }

        private void OnMapDelegateDetailsClick(object sender, MvxValueEventArgs<IMapAnnotation> e)
        {
            var venueAnnotation = e.Value as VenueAnnotation;
            if (venueAnnotation != null &&
                ViewModel.NavigateVenueCommand.CanExecute(venueAnnotation.Venue))
            {
                ViewModel.NavigateVenueCommand.Execute(venueAnnotation.Venue);
            }
        }

        private void InitializeCalEventViewController()
        {
            _editCalEventController = new EKEventEditViewController();
            _editCalEventController.EventStore = (EKEventStore)ViewModel.CurrentCalendarEvent.EventStore;
            _editCalEventController.Event = (EKEvent)ViewModel.CurrentCalendarEvent.EventObj;

            var viewDelegate = new OrgEventCalEditViewDelegate(ViewModel);
            _editCalEventController.EditViewDelegate = viewDelegate;

            PresentViewController(_editCalEventController, true, null);
        }

        private void DisposeCalEventViewController()
        {
            if (_editCalEventController != null)
            {
                _editCalEventController.DismissViewController(true, null);
                _editCalEventController.EditViewDelegate = null;
                _editCalEventController.Dispose();
                _editCalEventController = null;
            }
        }

        private void ShowHideListSettingsView(bool isShown)
        {
            if (isShown)
            {
                _listSettingsView = View.Subviews.
                    OfType<ListSettingsView>()
                    .FirstOrDefault();
                if (_listSettingsView == null)
                {
                    InitializeListSettingsView();

                    _listSettingsView.Alpha = 0;
                    View.Add(_listSettingsView);
                    UIView.BeginAnimations(null);
                    _listSettingsView.Alpha = 1;
                    UIView.CommitAnimations();
                }
            }
            else if (_listSettingsView != null)
            {
                UIView.Animate(
                    0.2, 
                    new NSAction(() => _listSettingsView.Alpha = 0),
                    new NSAction(_listSettingsView.RemoveFromSuperview));

                DisposeListSettingsView();
            }

            var tableSoure = VenuesAndShowsTableView.WeakDataSource as HiddenHeaderTableSource<Venue>;
            if (tableSoure != null)
            {
                tableSoure.IsAutohidingEnabled = !isShown;
            }
        }

        // TODO: Maybe to support showing on Top of headerView if there is no bottom space
        private float GetListSettingsTopMargin()
        {
            var headerLocation = 
                View.ConvertPointFromView(
                    _headerView.Frame.Location, 
                    VenuesAndShowsTableView);

            var result = headerLocation.Y + _headerView.Frame.Height;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0) &&
                ViewModel.Mode == OrgEventViewMode.List)
            {
                result -= TopLayoutGuide.Length;
            }

            return result;
        }

        private void InitializeListSettingsView()
        {
            _listSettingsView = ListSettingsView.Create();
            _listSettingsView.MarginTop = GetListSettingsTopMargin();

            SetDialogViewFullscreenFrame(_listSettingsView);

            _listSettingsView.IsGroupByLocation = ViewModel.IsGroupedByLocation;
            _listSettingsView.SortBy = ViewModel.SortBy;

            _listSettingsView.Initialize();

            _listSettingsView.GroupByLocationCommand = ViewModel.GroupByLocationCommand;
            _listSettingsView.SortByCommand = ViewModel.SortByCommand;
            _listSettingsView.CloseCommand = ViewModel.ShowHideListOptionsCommand;
        }

        private void DisposeListSettingsView()
        {
            if (_listSettingsView != null)
            {
                _listSettingsView.GroupByLocationCommand = null;
                _listSettingsView.SortByCommand = null;
                _listSettingsView.CloseCommand = null;
                _listSettingsView.Dispose();
                _listSettingsView = null;
            }
        }
       
        private void UpdateViewState(bool animated)
        {
            switch (ViewModel.Mode)
            {
                case OrgEventViewMode.Combo:
                    _modeButtonItem.CustomView = new UIView();
                    _customModeButtonItem.CustomView = _modeListButton;

                    MapFullscreenButton.VerticalIcon = ThemeIcons.Fullscreen;
                    MapFullscreenButton.UpdateState();

                    TablePanel.Hidden = false;
                    MapPanel.Hidden = false;

                    DeactivateSearchController();
                    InitializeMapView();
                    break;

                case OrgEventViewMode.Map:
                    _modeButtonItem.CustomView = new UIView();
                    _customModeButtonItem.CustomView = _modeListButton;

                    MapFullscreenButton.VerticalIcon = ThemeIcons.ExitFullscreen;
                    MapFullscreenButton.UpdateState();

                    TablePanel.Hidden = true;
                    MapPanel.Hidden = false;

                    DeactivateSearchController();
                    InitializeMapView();
                    break;

                case OrgEventViewMode.List:
                    _customModeButtonItem.CustomView = new UIView();
                    _modeButtonItem.CustomView = _modeMapButton;

                    TablePanel.Hidden = false;
                    MapPanel.Hidden = true;
                    break;
            }
                    
            UpdateNavBarState(animated);
            UpdateViewConstraints(animated);

            // hiding ListOptions on switching to any state
            if (ViewModel.ShowHideListOptionsCommand.CanExecute(false))
            {
                ViewModel.ShowHideListOptionsCommand.Execute(false);
            }
        }

        private void UpdateNavBarState(bool animated)
        {
            if (ViewModel.Mode == OrgEventViewMode.List)
            {
                NavBarManager.Instance.SetNavBarHidden(false, true, animated);

                if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                {
                    UIApplication.SharedApplication
                        .SetStatusBarStyle(UIStatusBarStyle.LightContent, animated);
                }
                else
                {
                    #pragma warning disable 618

                    WantsFullScreenLayout = false;
                    UIApplication.SharedApplication
                        .SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, animated);

                    #pragma warning restore 618
                }
            }
            else
            {
                NavBarManager.Instance.SetNavBarHidden(true, false, animated);

                if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                {
                    UIApplication.SharedApplication
                        .SetStatusBarStyle(UIStatusBarStyle.Default, animated);
                }
                else
                {
                    #pragma warning disable 618

                    WantsFullScreenLayout = true;
                    UIApplication.SharedApplication
                        .SetStatusBarStyle(UIStatusBarStyle.BlackTranslucent, animated);

                    #pragma warning restore 618
                }
            }
        }

        private void UpdateDayButtonState()
        {
            if (ViewModel.IsMultiday)
            {
                _dayButton.Hidden = false;
                _customDayButton.Hidden = false;
                _dayButton.SetTitle(ViewModel.CurrentDayTitle, UIControlState.Normal);
                _customDayButton.SetTitle(ViewModel.CurrentDayTitle, UIControlState.Normal);
            }
            else
            {
                _dayButton.Hidden = true;
                _customDayButton.Hidden = true;
                _dayButton.SetTitle(null, UIControlState.Normal);
                _customDayButton.SetTitle(null, UIControlState.Normal);
            }
        }

        private void UpdateTableViewOnShowExpanding(UITableView tableView)
        {
            var tableSoure = tableView.WeakDataSource as OrgEventTableSource;
            if (tableView.VisibleCells.Length == 0 ||
                tableSoure == null) return;

            foreach (var cell in tableView.VisibleCells.OfType<VenueShowCell>())
            {
                cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                cell.HeaderView = 
                    tableSoure.GetHeaderForShowCell(cell.IsExpanded, cell.DataContext);
            }

            if (!ViewModel.IsGroupedByLocation)
            {
                var previousOffset = tableView.ContentOffset;

                var previousIndex = _previousExpandedShow != null
                    ? tableSoure.GetItemIndex(_previousExpandedShow)
                    : null;

                var currentIndex = ViewModel.ExpandedShow != null
                    ? tableSoure.GetItemIndex(ViewModel.ExpandedShow)
                    : null;

                var delta = 
                    (ViewModel.ExpandedShow != null ? 1: -1) * 
                        VenueHeaderView.DefaultHeight -
                        (previousIndex != null && 
                            currentIndex != null && 
                                previousIndex.Section < currentIndex.Section
                            ? VenueShowCell.CalculateCellHeight(
                                tableView.Frame.Width,
                                true,
                                false,
                                _previousExpandedShow) + 25 // just a magic number, really
                            : 0);

                // Compensating the shift created by expanded headers or previous cells
                if (tableView.ScrollEnabled &&
                    previousOffset.Y + delta > 0)
                {
                    tableView.SetContentOffset(
                        new PointF(previousOffset.X, previousOffset.Y + delta), 
                        true);
                }
            }

            tableView.BeginUpdates();
            tableView.EndUpdates();
        }
    }

    #region HACK
    // HACK: To hide nav bar after iOS make it visible in next view
    public class ExtendedSearchDisplayController : UISearchDisplayController
    {
        public ExtendedSearchDisplayController(
            UISearchBar searchBar, 
            UIViewController viewController) 
            : base(searchBar, viewController) {}

        public override void SetActive(bool visible, bool animated)
        {
            var navCtr = SearchContentsController.NavigationController;
            if (navCtr.VisibleViewController is OrgEventView)
            {
                base.SetActive(visible, animated);
            }
            else
            {
                var previousHidden = NavBarManager.Instance.NativeHidden;
                base.SetActive(visible, animated);
                navCtr.SetNavigationBarHidden(previousHidden, false);
            }
        }
    }
    #endregion
}