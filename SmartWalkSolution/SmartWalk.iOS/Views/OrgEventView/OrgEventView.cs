using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private OrgEventHeaderView _headerView;
        private UIBarButtonItem _modeButton;
        private UISearchDisplayController _searchDisplayController;
        private UISwipeGestureRecognizer _swipeLeft;
        private bool _isMapViewInitialized;
        private bool _isAnimating;
        private PointF _tableContentOffset;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            VenuesMapView.Delegate = new OrgEventMapDelegate(ViewModel);

            InitializeToolBar();
            InitializeTableHeader();
            InitializeSearchDisplayController();
            InitializeGestures();

            UpdateViewState(false);
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
            }
        }

        // HACK: To persist table scroll offset
        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            _tableContentOffset = VenuesAndShowsTableView.ContentOffset;
        }

        // HACK: To persist table scroll offset
        public override void ViewWillAppear(bool animated)
        {
            if (_tableContentOffset != PointF.Empty)
            {
                VenuesAndShowsTableView.SetContentOffset(_tableContentOffset, false);
                _tableContentOffset = PointF.Empty;
            }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenuesAndShowsTableView);  
        }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.OrgEvent != null && ViewModel.OrgEvent.Info != null)
            {
                NavigationItem.Title = ViewModel.OrgEvent.Info.Date.ToShortDateString();
            }
        }

        protected override object CreateListViewSource()
        {
            var tableSource = new OrgEventTableSource(
                VenuesAndShowsTableView,
                ViewModel);

            this.CreateBinding(tableSource)
                .To((OrgEventViewModel vm) => vm.OrgEvent.Venues)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            if (propertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewState();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                if (!_isAnimating && ViewModel.Mode == OrgEventViewMode.Map)
                {
                    SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsGroupedByLocation))
            {
                VenuesAndShowsTableView.ReloadData();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                foreach (var cell in VenuesAndShowsTableView.VisibleCells.OfType<VenueShowCell>())
                {
                    cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                }

                VenuesAndShowsTableView.BeginUpdates();
                VenuesAndShowsTableView.EndUpdates();

                // TODO: check out how to know if search table is visible
                foreach (var cell in SearchDisplayController.SearchResultsTableView.VisibleCells.OfType<VenueShowCell>())
                {
                    cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                }

                SearchDisplayController.SearchResultsTableView.BeginUpdates();
                SearchDisplayController.SearchResultsTableView.EndUpdates();
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

        private void InitializeToolBar()
        {
            _modeButton = new UIBarButtonItem();
            _modeButton.Clicked += OnModeButtonClicked;

            NavigationItem.SetRightBarButtonItem(_modeButton, true);
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
            _headerView.SearchBarControl.WeakDelegate = this;

            VenuesAndShowsTableView.TableHeaderView = _headerView;
        }

        private void DisposeTableHeader()
        {
            if (_headerView != null)
            {
                VenuesAndShowsTableView.TableHeaderView = null;
                _headerView.SearchBarControl.WeakDelegate = null;
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
                    .To((OrgEventViewModel vm) => vm.OrgEvent.Venues)
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

                    var annotations = ViewModel.OrgEvent.Venues
                        .SelectMany(v => v.Info.Addresses
                            .Select(a => new VenueAnnotation(v, a))).ToArray();
                    var coordinates = GetAnnotationsCoordinates(annotations);

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

        private void SelectVenueMapAnnotation(Venue venue)
        {
            if (venue != null)
            {
                var annotation = VenuesMapView.Annotations
                .OfType<VenueAnnotation>()
                    .FirstOrDefault(an => Equals(an.Venue, venue));

                if (annotation != null)
                {
                    VenuesMapView.SetRegion(
                        MapUtil.CoordinateRegionForCoordinates(annotation.Coordinate), true);
                    VenuesMapView.SelectAnnotation(annotation, true);
                }
            }
            else if (VenuesMapView.SelectedAnnotations.Any())
            {
                foreach (var annotation in VenuesMapView.SelectedAnnotations)
                {
                    VenuesMapView.DeselectAnnotation(annotation, false);
                }

                var annotations = VenuesMapView.Annotations.OfType<VenueAnnotation>();
                var coordinates = GetAnnotationsCoordinates(annotations);
                VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), true);
            }
        }

        private CLLocationCoordinate2D[] GetAnnotationsCoordinates(IEnumerable<VenueAnnotation> annotations)
        {
            var coordinates = annotations
                .Select(va => va.Coordinate)
                    .Where(c => (long)c.Latitude != 0 && (long)c.Longitude != 0).ToArray();
            return coordinates;
        }
       
        private void UpdateViewState(bool isAnimated = true)
        {
            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                _modeButton.Title = "List";

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
                _modeButton.Title = "Map";

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
    }
}