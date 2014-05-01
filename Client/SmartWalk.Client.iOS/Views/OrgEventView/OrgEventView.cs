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
        private UISearchDisplayController _searchDisplayController;
        private EKEventEditViewController _editCalEventController;
        private ListSettingsView _listSettingsView;
        private UISwipeGestureRecognizer _swipeLeft;
        private bool _isMapViewInitialized;
        private bool _isAnimating;
        private bool _isBackOverriden;
        private PointF _tableContentOffset;
        private Show _previousExpandedShow;
        private ButtonBarButton _modeButtonList;
        private ButtonBarButton _modeButtonMap;
        private ButtonBarButton _moreButton;

        private NSTimer _timer;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeConstraints();
            InitializeToolBar();
            InitializeGestures();

            UpdateViewState(false);
        }

        // set header before items source is passed to table
        // for easier header auto-hiding
        protected override void OnBeforeSetListViewSource()
        {   
            InitializeTableHeader();
            InitializeSearchDisplayController();
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent == null)
            {
                DisposeToolBar();
                DisposeGestures();
                DisposeTableHeader();
                DisposeSearchDisplayController();
                DisposeMapView();
            }
        }

        // HACK: To persist table scroll offset
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            _tableContentOffset = VenuesAndShowsTableView.ContentOffset;
        }

        // TODO: Find another soltuion. It must be much simpler.
        // HACK: To persist table scroll offset on rotation and appearing
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

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
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            if (_listSettingsView != null)
            {
                SetDialogViewFullscreenFrame(_listSettingsView);
            }
        }

        protected override void OnNavigationBackClick()
        {
            if (_isBackOverriden)
            {
                if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
                {
                    ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
                }
            }
            else
            {
                base.OnNavigationBackClick();
            }
        }

        protected override void UpdateViewTitle()
        {
            base.UpdateViewTitle();

            if (_headerView != null)
            {
                _headerView.Title = ViewTitle;
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
            if (propertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewState(true);

                if (ViewModel.Mode == OrgEventViewMode.List)
                {
                    _isBackOverriden = false;
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                if ((!_isAnimating && ViewModel.Mode == OrgEventViewMode.Map) ||
                    ViewModel.SelectedVenueOnMap == null)
                {
                    SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                }

                _isBackOverriden = 
                    _isAnimating &&
                    ViewModel.Mode == OrgEventViewMode.Map &&
                    ViewModel.SelectedVenueOnMap != null;
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
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsListOptionsShown))
            {
                ShowHideListSettingsView(ViewModel.IsListOptionsShown);
            }
        }

        protected override void OnViewModelRefreshed()
        {
            _isMapViewInitialized = false;

            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                InitializeMapView();
            }

            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource;
            if (tableSource != null && !tableSource.IsHeaderViewHidden)
            {
                tableSource.ScrollOutHeader();
            }
        }

        protected override void OnLoadedViewStateUpdate()
        {
            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource;
            if (tableSource == null || tableSource.IsHeaderViewHidden)
            {
                base.OnLoadedViewStateUpdate();
            }
        }

        private void InitializeConstraints()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                View.RemoveConstraint(TablePanelTopConstaint);

                var views = new NSDictionary("topGuide", TopLayoutGuide, "view", TablePanel);
                var constraint = NSLayoutConstraint.FromVisualFormat("V:[topGuide]-0-[view]", 0, null, views);
                View.AddConstraints(constraint);

                View.RemoveConstraint(MapPanelTopConstraint);

                views = new NSDictionary("topGuide", TopLayoutGuide, "view", MapPanel);
                constraint = NSLayoutConstraint.FromVisualFormat("V:[topGuide]-0-[view]", 0, null, views);
                View.AddConstraints(constraint);
            }
        }

        private void InitializeToolBar()
        {
            _modeButtonList = ButtonBarUtil.Create(ThemeIcons.NavBarList, ThemeIcons.NavBarListLandscape);
            _modeButtonList.TouchUpInside += OnModeButtonClicked;

            _modeButtonMap = ButtonBarUtil.Create(ThemeIcons.NavBarMap, ThemeIcons.NavBarMapLandscape);
            _modeButtonMap.TouchUpInside += OnModeButtonClicked;

            _modeButton = new UIBarButtonItem(_modeButtonMap);
            var spacer = ButtonBarUtil.CreateSpacer();

            _moreButton = ButtonBarUtil.Create(ThemeIcons.NavBarMore, ThemeIcons.NavBarMoreLandscape);
            _moreButton.TouchUpInside += OnMoreButtonClicked;

            var moreBarButton = new UIBarButtonItem(_moreButton);
            NavigationItem.SetRightBarButtonItems(new [] {spacer, moreBarButton, _modeButton}, true);
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
            var actionSheet = ActionSheetUtil.CreateActionSheet(OnActionClicked);

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

            actionSheet.AddButton(Localization.CancelButton);

            actionSheet.CancelButtonIndex = actionSheet.ButtonCount - 1;

            actionSheet.ShowInView(View);
        }

        private void OnActionClicked(object sender, UIButtonEventArgs e)
        {
            var actionSheet = ((UIActionSheet)sender);
            actionSheet.Clicked -= OnActionClicked;

            switch (actionSheet.ButtonTitle(e.ButtonIndex))
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

        private void InitializeTableHeader()
        {
            _headerView = OrgEventHeaderView.Create();
            _headerView.Title = ViewTitle;
            _headerView.ShowOptionsCommand = ViewModel.ShowHideListOptionsCommand;

            VenuesAndShowsTableView.TableHeaderView = _headerView;
        }

        private void DisposeTableHeader()
        {
            if (_headerView != null)
            {
                _headerView.SearchBarControl.Dispose();
                _headerView.Dispose();
                _headerView = null;
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
            _swipeLeft = new UISwipeGestureRecognizer(rec => 
                {
                    if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.Map))
                    {
                        ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.Map);
                    }
                });

            _swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            TablePanel.AddGestureRecognizer(_swipeLeft);
        }

        private void DisposeGestures()
        {
            if (_swipeLeft != null)
            {
                TablePanel.RemoveGestureRecognizer(_swipeLeft);
                _swipeLeft.Dispose();
                _swipeLeft = null;
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

                    if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                    {
                        VenuesMapView.TintColor = Theme.MapTint;
                    }

                    if (!(VenuesMapView.WeakDelegate is MapDelegate))
                    {
                        var mapDelegate = new MapDelegate();
                        mapDelegate.DetailsClick += OnMapDelegateDetailsClick;
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
                mapDelegate.DetailsClick -= OnMapDelegateDetailsClick;
                mapDelegate.Dispose();
                VenuesMapView.WeakDelegate = null;
            }
        }

        private void SelectVenueMapAnnotation(Venue venue)
        {
            if (venue != null)
            {
                var annotation = 
                    VenuesMapView.Annotations
                        .OfType<VenueAnnotation>()
                            .FirstOrDefault(an => 
                                an.Venue.Info.Id == venue.Info.Id);

                if (annotation != null)
                {
                    VenuesMapView.SetRegion(
                        MapUtil.CoordinateRegionForCoordinates(annotation.Coordinate), true);
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

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
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
       
        private void UpdateViewState(bool isAnimated)
        {
            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                _modeButtonList.UpdateState();
                _modeButton.CustomView = _modeButtonList;

                var completeHandler = new NSAction(() => 
                    {
                        _isAnimating = false;

                        TablePanel.Hidden = true;
                        MapPanel.Hidden = false;

                        InitializeMapView();

                        if (ViewModel.SelectedVenueOnMap != null)
                        {
                            SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                        }
                    });

                if (isAnimated)
                {
                    _isAnimating = true;

                    UIView.Transition(
                        TablePanel, 
                        MapPanel, 
                        0.8, 
                        UIViewAnimationOptions.TransitionFlipFromRight | 
                        UIViewAnimationOptions.ShowHideTransitionViews, 
                        completeHandler);
                }
                else
                {
                    completeHandler();
                }
            }
            else
            {
                _modeButtonMap.UpdateState();
                _modeButton.CustomView = _modeButtonMap;

                var completeHandler = new NSAction(() => 
                    {
                        _isAnimating = false;

                        TablePanel.Hidden = false;
                        MapPanel.Hidden = true;
                    });

                if (isAnimated)
                {
                    _isAnimating = true;

                    UIView.Transition(
                        MapPanel, 
                        TablePanel, 
                        0.8,
                        UIViewAnimationOptions.TransitionFlipFromLeft | 
                        UIViewAnimationOptions.ShowHideTransitionViews,
                        completeHandler);
                }
                else
                {
                    completeHandler();
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