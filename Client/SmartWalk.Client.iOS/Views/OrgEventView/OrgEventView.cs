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
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private OrgEventHeaderView _headerView;
        private UIBarButtonItem _modeButton;
        private UIBarButtonItem _customModeButton;
        private UISearchDisplayController _searchDisplayController;
        private EKEventEditViewController _editCalEventController;
        private ListSettingsView _listSettingsView;
        private bool _isMapViewInitialized;
        private PointF _tableContentOffset;
        private Show _previousExpandedShow;
        private ButtonBarButton _modeButtonList;
        private ButtonBarButton _modeButtonMap;
        private ButtonBarButton _moreButton;
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

            InitializeStyle();
            InitializeToolBar();
            InitializeGestures();

            UpdateViewState(false);
        }

        // TODO: Find another soltuion. It must be much simpler.
        // HACK: To persist table scroll offset on rotation and appearing
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetStatusBarVisibility(true, animated);
            UpdateNavBarState(animated);

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
                new [] {_modeButtonList, _modeButtonMap, MapFullscreenButton});
        }

        // HACK: To persist table scroll offset
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            _tableContentOffset = VenuesAndShowsTableView.ContentOffset;
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

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
                new [] {_modeButtonList, _modeButtonMap, MapFullscreenButton});

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
            var tableSource = new OrgEventTableSource(
                VenuesAndShowsTableView,
                ViewModel);

            this.CreateBinding(tableSource)
                .To<OrgEventViewModel>(vm => vm.OrgEvent.Venues)
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
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                if (ViewModel.Mode == OrgEventViewMode.Map || 
                    ViewModel.Mode == OrgEventViewMode.Combo ||
                    ViewModel.SelectedVenueOnMap == null)
                {
                    SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsGroupedByLocation) ||
                propertyName == ViewModel.GetPropertyName(vm => vm.SortBy))
            {
                VenuesAndShowsTableView.ReloadData();

                if (SearchDisplayController.SearchResultsTableView.Superview != null &&
                    !SearchDisplayController.SearchResultsTableView.Hidden)
                {
                    SearchDisplayController.SearchResultsTableView.ReloadData();
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
        }

        protected override void OnViewModelRefreshed()
        {
            base.OnViewModelRefreshed();

            _isMapViewInitialized = false;

            if (ViewModel.Mode == OrgEventViewMode.Map ||
                ViewModel.Mode == OrgEventViewMode.Combo)
            {
                InitializeMapView();
            }

            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource;
            if (tableSource != null && !tableSource.IsHeaderViewHidden)
            {
                tableSource.ScrollOutHeader();
            }
        }

        // set header before items source is passed to table
        // for easier header auto-hiding
        protected override void OnBeforeSetListViewSource()
        {   
            InitializeTableHeader();
            InitializeSearchDisplayController();
        }

        protected override void OnLoadedViewStateUpdate()
        {
            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource;
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
        }

        protected override void OnInitializeCustomNavBarItems(List<UIBarButtonItem> navBarItems)
        {
            base.OnInitializeCustomNavBarItems(navBarItems);

            _customModeButton = new UIBarButtonItem();

            navBarItems.Add(_customModeButton);
        }

        private void InitializeStyle()
        {
            MapFullscreenButton.IsSemiTransparent = true;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                VenuesMapView.TintColor = Theme.MapTint;
            }
        }

        private void InitializeToolBar()
        {
            _modeButtonList = ButtonBarUtil.Create(ThemeIcons.NavBarList, ThemeIcons.NavBarListLandscape, true);
            _modeButtonList.TouchUpInside += OnModeButtonClicked;

            _modeButtonMap = ButtonBarUtil.Create(ThemeIcons.NavBarMap, ThemeIcons.NavBarMapLandscape);
            _modeButtonMap.TouchUpInside += OnModeButtonClicked;

            _modeButton = new UIBarButtonItem();
            var gap = ButtonBarUtil.CreateGapSpacer();

            _moreButton = ButtonBarUtil.Create(ThemeIcons.NavBarMore, ThemeIcons.NavBarMoreLandscape);
            _moreButton.TouchUpInside += OnMoreButtonClicked;

            var moreBarButton = new UIBarButtonItem(_moreButton);
            NavigationItem.SetRightBarButtonItems(new [] {gap, moreBarButton, _modeButton}, true);
        }

        private void DisposeToolBar()
        {
            if (_modeButtonList != null)
            {
                _modeButtonList.TouchUpInside -= OnModeButtonClicked;
            }

            if (_modeButtonMap != null)
            {
                _modeButtonMap.TouchUpInside -= OnModeButtonClicked;
            }

            if (_moreButton != null)
            {
                _moreButton.TouchUpInside -= OnMoreButtonClicked;
            }
        }

        private void OnModeButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.SwitchModeCommand.CanExecute(null))
            {
                ViewModel.SwitchModeCommand.Execute(null);
            }
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            ShowActionSheet();
        }

        partial void OnMapFullscreenTouchUpInside (NSObject sender)
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
                _headerView.SearchBarControl.IsListOptionsVisible = ViewModel.IsListOptionsAvailable;
            }
        }

        private void InitializeSearchDisplayController()
        {
            _searchDisplayController = 
                new UISearchDisplayController(_headerView.SearchBarControl, this);

            var searchDelegate = new OrgEventSearchDelegate(ViewModel);
            _searchDisplayController.Delegate = searchDelegate;

            this.CreateBinding(searchDelegate)
                .For(p => p.ItemsSource)
                    .To<OrgEventViewModel>(vm => vm.OrgEvent.Venues)
                    .Apply();
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
                VenuesAndShowsTableView.ScrollToRow(
                    index, 
                    UITableViewScrollPosition.Top,
                    true);
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
                    _modeButton.CustomView = new UIView();
                    _customModeButton.CustomView = _modeButtonList;

                    MapFullscreenButton.VerticalIcon = ThemeIcons.Fullscreen;
                    MapFullscreenButton.UpdateState();

                    TablePanel.Hidden = false;
                    MapPanel.Hidden = false;

                    InitializeMapView();
                    break;

                case OrgEventViewMode.Map:
                    _modeButton.CustomView = new UIView();
                    _customModeButton.CustomView = _modeButtonList;

                    MapFullscreenButton.VerticalIcon = ThemeIcons.ExitFullscreen;
                    MapFullscreenButton.UpdateState();

                    TablePanel.Hidden = true;
                    MapPanel.Hidden = false;

                    InitializeMapView();
                    break;

                case OrgEventViewMode.List:
                    _customModeButton.CustomView = new UIView();
                    _modeButton.CustomView = _modeButtonMap;

                    TablePanel.Hidden = false;
                    MapPanel.Hidden = true;
                    break;
            }
                    
            UpdateNavBarState(animated);
            UpdateViewConstraints(animated);
        }

        private void UpdateNavBarState(bool animated)
        {
            if (ViewModel.Mode == OrgEventViewMode.List)
            {
                NavBarManager.Instance.SetNavBarVisibility(true, false, animated);

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
                NavBarManager.Instance.SetNavBarVisibility(false, true, animated);

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

        private void UpdateTableViewOnShowExpanding(UITableView tableView)
        {
            if (tableView.VisibleCells.Length == 0 ||
                tableView.WeakDataSource == null) return;

            foreach (var cell in tableView.VisibleCells.OfType<VenueShowCell>())
            {
                cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                cell.IsHighlighted = cell.IsExpanded && !ViewModel.IsGroupedByLocation;
            }

            if (!ViewModel.IsGroupedByLocation)
            {
                var tableSoure = (OrgEventTableSource)tableView.WeakDataSource;
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

                if (previousIndex != null)
                {
                    tableView.ReloadSections(
                        NSIndexSet.FromIndex(previousIndex.Section),
                        UITableViewRowAnimation.Fade);
                }

                if (currentIndex != null)
                {
                    tableView.ReloadSections(
                        NSIndexSet.FromIndex(currentIndex.Section), 
                        UITableViewRowAnimation.Fade);
                }
            }

            tableView.BeginUpdates();
            tableView.EndUpdates();
        }
    }
}