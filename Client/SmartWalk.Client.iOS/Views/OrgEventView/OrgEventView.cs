using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using CoreLocation;
using Foundation;
using MapKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.OrgEventView;
using SmartWalk.Client.iOS.Views.OrgView;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private readonly MKMapSize MapMargin = new MKMapSize(3000, 3000);

        private OrgEventHeaderView _headerView;
        private UISearchDisplayController _searchDisplayController;
        private ListSettingsView _listSettingsView;
        private bool _isMapViewInitialized;
        private ButtonBarButton _modeButton;
        private ButtonBarButton _dayButton;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
        }

        private bool HasData
        {
            get { return ViewModel != null && ViewModel.OrgEvent != null; }
        }

        private OrgEventViewMode CurrentMode
        {
            get { return HasData ? ViewModel.Mode : OrgEventViewMode.List; }
        }

        private bool IsInSearch
        {
            get
            { 
                return _searchDisplayController != null &&
                    _searchDisplayController.Active;
            }
        }

        private bool IsNavBarTransparent 
        {
            get 
            {
                return CurrentMode != OrgEventViewMode.List || !HasData;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.ZoomToVenue += OnZoomToVenue;
            ViewModel.ScrollToVenue += OnScrollToVenue;
            ViewModel.ScrollToShow += OnScrollToShow;

            InitializeStyle();
            InitializeListSettingsView();

            UpdateViewState(false);
            UpdateDayButtonState();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateTableViewInset();
            UpdateViewConstraints(false); // to get a right golden-ratio in time
            UpdateNavBarState(animated);
            UpdateButtonsFrameOnRotation();
            UpdateDayButtonState();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel.ZoomToVenue -= OnZoomToVenue;
                ViewModel.ScrollToVenue -= OnScrollToVenue;

                DisposeToolBar();
                DisposeTableHeader();
                DisposeSearchDisplayController();
                DisposeMapView();
                DisposeListSettingsView();
            }
        }

        public override void WillAnimateRotation(
            UIInterfaceOrientation toInterfaceOrientation, 
            double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            UpdateTableViewInset();
            UpdateDayButtonState();
            UpdateViewConstraints(true);
            UpdateButtonsFrameOnRotation();
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();

            UpdateTableViewInset();
            UpdateViewConstraints(false);
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            if (CurrentMode == OrgEventViewMode.List && HasData)
            {
                return UIStatusBarStyle.LightContent;
            }

            return UIStatusBarStyle.Default;
        }

        public override bool PrefersStatusBarHidden()
        {
            return IsInSearch;
        }

        protected override void SetNavBarHidden(bool hidden, bool animated)
        {
            if (IsInSearch)
            {
                base.SetNavBarHidden(true, animated);
            }
            else
            {
                base.SetNavBarHidden(hidden, animated);
            }
        }

        private void UpdateViewConstraints(bool animated)
        {
            switch (CurrentMode)
            {
                case OrgEventViewMode.Combo:
                    VenuesAndShowsTableView.RemoveConstraint(TableHeightConstraint);

                    if (!MapPanel.Constraints.Contains(MapHeightConstraint))
                    {
                        MapPanel.AddConstraint(MapHeightConstraint);
                    }

                    View.UpdateConstraint(
                        () => MapHeightConstraint.Constant = ScreenUtil.GetGoldenRatio(View.Frame.Height),
                        animated);

                    MapToListSettingsConstraint.Constant = 0;
                    ListSettingsHeightConstraint.Constant = 0;
                    ListSettingsToTableConstraint.Constant = -VenuesAndShowsTableView.ContentInset.Top;
                    break;

                case OrgEventViewMode.Map:
                    MapPanel.RemoveConstraint(MapHeightConstraint);

                    if (!VenuesAndShowsTableView.Constraints.Contains(TableHeightConstraint))
                    {
                        VenuesAndShowsTableView.AddConstraint(TableHeightConstraint);
                    }

                    View.UpdateConstraint(
                        () => TableHeightConstraint.Constant = 0,
                        animated);

                    MapToListSettingsConstraint.Constant = 0;
                    ListSettingsHeightConstraint.Constant = 0;
                    ListSettingsToTableConstraint.Constant = 0;
                    break;

                case OrgEventViewMode.List:
                    VenuesAndShowsTableView.RemoveConstraint(TableHeightConstraint);

                    if (!MapPanel.Constraints.Contains(MapHeightConstraint))
                    {
                        MapPanel.AddConstraint(MapHeightConstraint);
                    }

                    View.UpdateConstraint(
                        () => MapHeightConstraint.Constant = 0,
                        animated);

                    MapToListSettingsConstraint.Constant = NavBarManager.NavBarHeight;
                    ListSettingsHeightConstraint.Constant = ListSettingsView.DefaultHeight;
                    ListSettingsToTableConstraint.Constant = -NavBarManager.NavBarHeight - ListSettingsView.DefaultHeight;
                    break;
            }

            var size = ScreenUtil.IsVerticalOrientation 
                ? ButtonBarButton.DefaultVerticalSize
                : ButtonBarButton.DefaultLandscapeSize;
            FullscreenWidthConstraint.Constant = size.Width;
            FullscreenHeightConstraint.Constant = size.Height;
        }

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(VenuesAndShowsTableView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override IListViewSource CreateListViewSource()
        {
            var tableSource = new OrgEventTableSource(VenuesAndShowsTableView, ViewModel, ListSettingsContainer);

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
                UpdateDayButtonState();
                UpdateViewState(false);
                ReloadMap();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SelectedVenueOnMap))
            {
                if (CurrentMode == OrgEventViewMode.Map ||
                    CurrentMode == OrgEventViewMode.Combo ||
                    ViewModel.SelectedVenueOnMap == null)
                {
                    SelectVenueMapAnnotation(ViewModel.SelectedVenueOnMap);
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                UpdateTableViewOnShowExpanding(VenuesAndShowsTableView);

                if (IsInSearch &&
                    _searchDisplayController.SearchResultsTableView.Superview != null &&
                    !_searchDisplayController.SearchResultsTableView.Hidden)
                {
                    UpdateTableViewOnShowExpanding(_searchDisplayController.SearchResultsTableView);
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsListOptionsAvailable))
            {
                UpdateTableHeaderState(HasData);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsMultiday) ||
                propertyName == ViewModel.GetPropertyName(vm => vm.CurrentDay))
            {
                UpdateDayButtonState();
            }
        }

        protected override void OnViewModelRefreshed(bool hasData, bool pullToRefresh)
        {
            base.OnViewModelRefreshed(hasData, pullToRefresh);

            UpdateTableHeaderState(hasData);
            UpdateTableViewInset();
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

        protected override void OnInitializingActionSheet(List<string> titles)
        {
            if (ViewModel.NavigateOrgEventInfoCommand.CanExecute(null))
            {
                titles.Add(Localization.EventInfo);
            }

            if (ViewModel.NavigateOrgCommand.CanExecute(null))
            {
                titles.Add(Localization.OrganizerInfo);
            }

            if (ViewModel.SwitchMapTypeCommand.CanExecute(null))
            {
                titles.Add(ViewModel.CurrentMapType.GetMapTypeButtonLabel());
            }

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                titles.Add(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                titles.Add(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.EventInfo:
                    if (ViewModel.NavigateOrgEventInfoCommand.CanExecute(null))
                    {
                        ViewModel.NavigateOrgEventInfoCommand.Execute(null);
                    }
                    break;

                case Localization.OrganizerInfo:
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

        protected override void OnInitializeNavBarItems(List<UIBarButtonItem> navBarItems)
        {
            base.OnInitializeNavBarItems(navBarItems);

            // Day Button
            _dayButton = ButtonBarUtil.Create(SemiTransparentType.Dark);
            _dayButton.LineBreakMode = UILineBreakMode.WordWrap;
            _dayButton.TouchUpInside += OnDayButtonClicked;

            var dayButtonItem = new UIBarButtonItem();
            dayButtonItem.CustomView = _dayButton;

            navBarItems.Add(dayButtonItem);

            // Mode (List, Map, Combined) Button
            _modeButton = ButtonBarUtil.Create(
                ThemeIcons.List, 
                ThemeIcons.ListLandscape, 
                SemiTransparentType.Dark);
            _modeButton.TouchUpInside += OnModeButtonClicked;

            var modeButtonItem = new UIBarButtonItem();
            modeButtonItem.CustomView = _modeButton;

            navBarItems.Add(modeButtonItem);
        }

        private void DisposeToolBar()
        {
            if (_modeButton != null)
            {
                _modeButton.TouchUpInside -= OnModeButtonClicked;
            }

            if (_dayButton != null)
            {
                _dayButton.TouchUpInside -= OnDayButtonClicked;
            }
        }

        private void InitializeStyle()
        {
            MapFullscreenButton.SemiTransparentType = SemiTransparentType.Dark;
            MapFullscreenButton.UpdateState();

            VenuesMapView.TintColor = ThemeColors.Metadata;
            VenuesMapView.FixLegalLabel();
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

            UpdateTableHeaderState(HasData);

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

        private void UpdateTableHeaderState(bool hasData)
        {
            if (_headerView != null)
            {
                _headerView.IsListOptionsVisible = ViewModel.IsListOptionsAvailable;
                _headerView.SetHidden(!hasData, false);
            }
        }

        private void InitializeSearchDisplayController()
        {
            _searchDisplayController = 
                new OrgEventSearchDisplayController(_headerView.SearchBarControl, this);
            _searchDisplayController.Delegate = 
                new OrgEventSearchDelegate(VenuesAndShowsTableView, ViewModel);

            var searchTableSource = new OrgEventTableSource(
                    _searchDisplayController.SearchResultsTableView, ViewModel)
                {
                    IsSearchSource = true
                };

            this.CreateBinding(searchTableSource)
                .For(ts => ts.ItemsSource)
                .To<OrgEventViewModel>(vm => vm.SearchListItems)
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

        private void ReloadMap()
        {
            _isMapViewInitialized = false;

            if (CurrentMode == OrgEventViewMode.Map ||
                CurrentMode == OrgEventViewMode.Combo)
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

                    VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates, MapMargin), false);
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
                VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates, MapMargin), true);
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

        private void OnZoomToVenue(object sender, MvxValueEventArgs<Venue> e)
        {
            var annotation = GetAnnotationByVenue(ViewModel.SelectedVenueOnMap);

            var shiftedCoord = annotation.Coordinate;
            shiftedCoord.Latitude += 0.0008f;

            VenuesMapView.SetRegion(
                MapUtil.CoordinateRegionForCoordinates(shiftedCoord), true);
        }

        private void OnScrollToVenue(object sender, MvxValueEventArgs<Venue> e)
        {
            var tableSource = VenuesAndShowsTableView.WeakDelegate as OrgEventTableSource;
            if (tableSource != null)
            {
                var index = tableSource.GetItemIndex(e.Value);
                if (index != null)
                {
                    VenuesAndShowsTableView.ScrollToRow(
                        index, 
                        UITableViewScrollPosition.Top,
                        true);
                }
            }
        }

        private void OnScrollToShow(object sender, MvxValueEventArgs<Show> e)
        {
            var tableSource = VenuesAndShowsTableView.WeakDelegate as OrgEventTableSource;
            if (tableSource != null)
            {
                var index = tableSource.GetItemIndex(e.Value);
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

        private void InitializeListSettingsView()
        {
            if (_listSettingsView == null)
            {
                _listSettingsView = ListSettingsView.Create();
                _listSettingsView.Frame = ListSettingsContainer.Bounds;
                ListSettingsContainer.Content = _listSettingsView;

                _listSettingsView.SortBy = ViewModel.SortBy;
                _listSettingsView.SortByCommand = ViewModel.SortByCommand;
            }
        }

        private void DisposeListSettingsView()
        {
            if (_listSettingsView != null)
            {
                _listSettingsView.SortByCommand = null;
                _listSettingsView.Dispose();
                _listSettingsView = null;
                ListSettingsContainer.Content = null;
            }
        }
       
        private void UpdateViewState(bool animated)
        {
            switch (CurrentMode)
            {
                case OrgEventViewMode.Combo:
                    _modeButton.Hidden = false;
                    _modeButton.VerticalIcon = ThemeIcons.List;
                    _modeButton.LandscapeIcon = ThemeIcons.ListLandscape;
                    _modeButton.UpdateState();

                    MapFullscreenButton.VerticalIcon = ThemeIcons.Fullscreen;
                    MapFullscreenButton.LandscapeIcon = ThemeIcons.FullscreenLandscape;
                    MapFullscreenButton.UpdateState();

                    VenuesAndShowsTableView.Hidden = false;
                    MapPanel.Hidden = false;
                    MapFullscreenButton.SetHidden(false, animated);
                    ListSettingsContainer.SetHidden(true, animated);

                    SetScrollToHideActive(false);
                    DeactivateSearchController();
                    InitializeMapView();
                    break;

                case OrgEventViewMode.Map:
                    _modeButton.Hidden = false;
                    _modeButton.VerticalIcon = ThemeIcons.List;
                    _modeButton.LandscapeIcon = ThemeIcons.ListLandscape;
                    _modeButton.UpdateState();

                    MapFullscreenButton.VerticalIcon = ThemeIcons.ExitFullscreen;
                    MapFullscreenButton.LandscapeIcon = ThemeIcons.ExitFullscreenLandscape;
                    MapFullscreenButton.UpdateState();

                    VenuesAndShowsTableView.SetHidden(true, animated);
                    MapPanel.Hidden = false;
                    MapFullscreenButton.SetHidden(false, animated);
                    ListSettingsContainer.SetHidden(true, animated);

                    SetScrollToHideActive(false);
                    DeactivateSearchController();
                    InitializeMapView();
                    break;

                case OrgEventViewMode.List:
                    _modeButton.Hidden = !HasData;
                    _modeButton.VerticalIcon = ThemeIcons.Map;
                    _modeButton.LandscapeIcon = ThemeIcons.MapLandscape;
                    _modeButton.UpdateState();

                    if (HasData)
                    {
                        VenuesAndShowsTableView.Hidden = false;
                    }

                    MapPanel.SetHidden(true, animated);
                    MapFullscreenButton.SetHidden(true, animated);
                    ListSettingsContainer.SetHidden(!HasData, animated);

                    SetScrollToHideActive(true);
                    break;
            }

            UpdateNavBarState(animated);
            UpdateDayButtonState();
            UpdateTableViewInset();
            UpdateViewConstraints(animated);
            SetNeedStatusBarUpdate(animated);
        }

        private void UpdateNavBarState(bool animated)
        {
            if (IsNavBarTransparent)
            {
                SetNavBarTransparent(SemiTransparentType.Dark, animated);
            }
            else
            {
                SetNavBarTransparent(SemiTransparentType.None, animated);
            }
        }

        private void UpdateDayButtonState()
        {
            var dateString = ViewModel.OrgEvent.GetOrgEventDateString(
                ThemeColors.ContentDarkText,
                ViewModel.CurrentDay,
                ScreenUtil.IsVerticalOrientation);

            _dayButton.SetAttributedTitle(dateString, UIControlState.Normal);
            _dayButton.Hidden = dateString.Length <= 0;
            _dayButton.Enabled = ViewModel.IsMultiday;

            // adjust due to long date text 
            if (ViewModel.IsMultiday && ViewModel.CurrentDay == null)
            {
                _dayButton.TitleEdgeInsets = new UIEdgeInsets(-3, 0.5f, 0, 0);
            }
            else
            {
                _dayButton.TitleEdgeInsets = UIEdgeInsets.Zero;
            }
        }

        private void UpdateTableViewOnShowExpanding(UITableView tableView)
        {
            var tableSoure = tableView.WeakDataSource as OrgEventTableSource;
            if (tableView.VisibleCells.Length == 0 || tableSoure == null) return;

            tableView.ExpandShowCell(ViewModel.ExpandedShow);
            tableView.BeginUpdates();
            tableView.EndUpdates();
        }

        private void UpdateTableViewInset()
        {
            if (IsInSearch) return;

            if (HasData && ViewModel.Mode == OrgEventViewMode.List)
            {
                var topInset = NavBarManager.NavBarHeight + 
                    (ListSettingsContainer.Hidden ? 0 : ListSettingsView.DefaultHeight);
                var previousOffset = VenuesAndShowsTableView.ContentOffset;
                var delta = VenuesAndShowsTableView.ContentInset.Top - topInset;

                VenuesAndShowsTableView.ContentInset = new UIEdgeInsets(topInset, 0, 0, 0);
                VenuesAndShowsTableView.ScrollIndicatorInsets =
                    VenuesAndShowsTableView.ContentInset;

                VenuesAndShowsTableView.ContentOffset =
                    new CGPoint(previousOffset.X, previousOffset.Y + delta);
            }
            else 
            {
                VenuesAndShowsTableView.ContentInset = UIEdgeInsets.Zero;
                VenuesAndShowsTableView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            }
        }

        private void UpdateButtonsFrameOnRotation()
        {
            ButtonBarUtil.UpdateButtonsFrameOnRotation(new [] { MapFullscreenButton });
        }

        private void SetScrollToHideActive(bool isActive)
        {
            var tableSource = VenuesAndShowsTableView.Source as OrgEventTableSource;
            if (tableSource != null)
            {
                tableSource.IsScrollToHideActive = isActive;
            }
        }
    }

    public static class ShowsTableViewExtensions
    {
        public static void ExpandShowCell(this UITableView tableView, Show show)
        {
            var showCells = tableView.VisibleCells.OfType<VenueShowCell>().ToArray();
            foreach (var cell in showCells)
            {
                var isExpanded = Equals(cell.DataContext.Show, show);
                var previousIsExpanded = cell.IsExpanded;
                cell.SetIsExpanded(isExpanded, true);

                var index = Array.IndexOf(showCells, cell);
                if (isExpanded != previousIsExpanded && index > 0)
                {
                    showCells[index - 1].SetIsBeforeExpanded(isExpanded, true);
                }
            }
        }
    }
}