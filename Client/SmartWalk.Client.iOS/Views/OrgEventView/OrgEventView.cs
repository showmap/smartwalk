using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.CoreLocation;
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
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private OrgEventHeaderView _headerView;
        private UIBarButtonItem _modeButton;
        private UISearchDisplayController _searchDisplayController;
        private UISwipeGestureRecognizer _swipeLeft;
        private bool _isMapViewInitialized;
        private bool _isAnimating;
        private bool _isBackOverriden;
        private PointF _tableContentOffset;
        private Show _previousExpandedShow;
        private UIButton _modeButtonList;
        private UIButton _modeButtonMap;

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

        protected override string GetViewTitle()
        {
            if (ViewModel.OrgEvent != null && ViewModel.OrgEvent.StartTime.HasValue)
            {
                return string.Format("{0:d MMMM yyyy}", ViewModel.OrgEvent.StartTime.Value);
            }

            return null;
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

                _isBackOverriden = _isAnimating && 
                    ViewModel.Mode == OrgEventViewMode.Map &&
                    ViewModel.SelectedVenueOnMap != null;
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsGroupedByLocation))
            {
                VenuesAndShowsTableView.ReloadData();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                UpdateTableViewOnShowExpanding(VenuesAndShowsTableView);

                if (SearchDisplayController.SearchResultsTableView.Superview != null)
                {
                    UpdateTableViewOnShowExpanding(SearchDisplayController.SearchResultsTableView);
                }

                _previousExpandedShow = ViewModel.ExpandedShow;
            }
        }

        protected override void OnViewModelRefreshed()
        {
            _isMapViewInitialized = false;

            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                InitializeMapView();
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

            NavigationItem.SetRightBarButtonItems(new [] {spacer, _modeButton}, true);
        }

        private void DisposeToolBar()
        {
            if (_modeButton != null)
            {
                _modeButton.Clicked -= OnModeButtonClicked;
            }
        }

        private void OnModeButtonClicked(object sender, EventArgs e)
        {
            if (ViewModel.SwitchModeCommand.CanExecute(null))
            {
                ViewModel.SwitchModeCommand.Execute(null);
            }
        }

        private void InitializeTableHeader()
        {
            _headerView = OrgEventHeaderView.Create();
            _headerView.GroupByLocationCommand = ViewModel.GroupByLocationCommand;

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
       
        private void UpdateViewState(bool isAnimated)
        {
            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
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
            foreach (var cell in tableView.VisibleCells.OfType<VenueShowCell>())
            {
                cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
            }

            if (!ViewModel.IsGroupedByLocation)
            {
                var tableSoure = (OrgEventTableSource)tableView.WeakDataSource;

                if (_previousExpandedShow != null)
                {
                    tableView.ReloadSections(
                        NSIndexSet.FromIndex(
                            tableSoure.GetSectionIndexByShow(_previousExpandedShow)),
                        UITableViewRowAnimation.Fade);
                }

                if (ViewModel.ExpandedShow != null)
                {
                    tableView.ReloadSections(
                        NSIndexSet.FromIndex(
                            tableSoure.GetSectionIndexByShow(ViewModel.ExpandedShow)),
                        UITableViewRowAnimation.Fade);
                }
            }

            tableView.BeginUpdates();
            tableView.EndUpdates();
        }
    }
}