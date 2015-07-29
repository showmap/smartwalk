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

        private OrgEventScrollToHideUIManager _scrollToHideManager;
        private ListSettingsView _listSettingsView;
        private bool _isMapViewInitialized;
        private ButtonBarButton _modeButton;
        private ButtonBarButton _dayButton;
        private UIBarButtonItem _dayButtonItem;
        private UIBarButtonItem _modeButtonItem;     
        private UITapGestureRecognizer _searchTableTapGesture;
        private bool _favoritesInvalidated;
        private bool _isAnimating;
        private event EventHandler AnimationFinished;

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
                return ViewModel != null && ViewModel.IsInSearch;
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
            ViewModel.FavoritesManager.FavoritesUpdated += OnFavoritesUpdated;

            InitializeStyle();
            InitializeListSettingsView();
            InitializeSearch();
            InitializeGestures();

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
            UpdateFavories();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel.ZoomToVenue -= OnZoomToVenue;
                ViewModel.ScrollToVenue -= OnScrollToVenue;
                ViewModel.FavoritesManager.FavoritesUpdated -= OnFavoritesUpdated;

                DisposeToolBar();
                DisposeMapView();
                DisposeListSettingsView();
                DisposeGestures();
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
            if (CurrentMode == OrgEventViewMode.List && HasData && !IsInSearch)
            {
                return UIStatusBarStyle.LightContent;
            }

            return UIStatusBarStyle.Default;
        }

        public override bool PrefersStatusBarHidden()
        {
            return false;
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
            _isAnimating = animated;
            var animationFinish = new Action(() =>
                {
                    _isAnimating = false;
                    if (AnimationFinished != null) AnimationFinished(this, EventArgs.Empty);
                });

            switch (CurrentMode)
            {
                case OrgEventViewMode.Combo:
                    VenuesAndShowsTableView.RemoveConstraint(TableHeightConstraint);

                    if (!View.Constraints.Contains(SearchTableTopConstraint))
                    {
                        View.AddConstraint(SearchTableTopConstraint);
                    }

                    if (!MapPanel.Constraints.Contains(MapHeightConstraint))
                    {
                        MapPanel.AddConstraint(MapHeightConstraint);
                    }

                    View.UpdateConstraint(
                        () => MapHeightConstraint.Constant = ScreenUtil.GetGoldenRatio(View.Frame.Height),
                        animated, animationFinish);

                    MapToListSettingsConstraint.Constant = 0;
                    ListSettingsHeightConstraint.Constant = 0;
                    ListSettingsToTableConstraint.Constant = 0;
                    break;

                case OrgEventViewMode.Map:
                    MapPanel.RemoveConstraint(MapHeightConstraint);
                    View.RemoveConstraint(SearchTableTopConstraint);

                    if (!VenuesAndShowsTableView.Constraints.Contains(TableHeightConstraint))
                    {
                        VenuesAndShowsTableView.AddConstraint(TableHeightConstraint);
                    }

                    View.UpdateConstraint(
                        () => TableHeightConstraint.Constant = 0,
                        animated, animationFinish);

                    MapToListSettingsConstraint.Constant = 0;
                    ListSettingsHeightConstraint.Constant = 0;
                    ListSettingsToTableConstraint.Constant = 0;
                    break;

                case OrgEventViewMode.List:
                    VenuesAndShowsTableView.RemoveConstraint(TableHeightConstraint);

                    if (!View.Constraints.Contains(SearchTableTopConstraint))
                    {
                        View.AddConstraint(SearchTableTopConstraint);
                    }

                    if (!MapPanel.Constraints.Contains(MapHeightConstraint))
                    {
                        MapPanel.AddConstraint(MapHeightConstraint);
                    }

                    View.UpdateConstraint(
                        () =>
                        {
                            MapHeightConstraint.Constant = 0;

                            if (IsInSearch)
                            {
                                MapToListSettingsConstraint.Constant = UIConstants.StatusBarHeight;
                                ListSettingsHeightConstraint.Constant = 0;
                                ListSettingsToTableConstraint.Constant = 0;
                            }
                            else
                            {
                                MapToListSettingsConstraint.Constant = NavBarManager.NavBarHeight;
                                ListSettingsHeightConstraint.Constant = ListSettingsView.DefaultHeight;
                                ListSettingsToTableConstraint.Constant = -NavBarManager.NavBarHeight -
                                    ListSettingsView.DefaultHeight;
                            }
                        },
                        animated, animationFinish);
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
            _scrollToHideManager = new OrgEventScrollToHideUIManager(
                VenuesAndShowsTableView, ListSettingsContainer);

            var tableSource = new OrgEventTableSource(VenuesAndShowsTableView, 
                ViewModel, _scrollToHideManager);

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
                UpdateTableViewInset();
                if (!ViewModel.IsInSearch)
                {
                    ScrollViewToTop(false);
                }

                UpdateViewState(true);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SortBy))
            {
                UpdateFavoritesUnavailableState();
                ScrollViewToTop(false);
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

                if (IsInSearch && !SearchTableView.Hidden)
                {
                    UpdateTableViewOnShowExpanding(SearchTableView);
                }
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsMultiday) ||
                     propertyName == ViewModel.GetPropertyName(vm => vm.CurrentDay))
            {
                UpdateDayButtonState();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.SearchListItems))
            {
                UpdateSearchTableViewState();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ShowOnlyFavorites))
            {
                UpdateFavoritesUnavailableState();
                ScrollViewToTop(false);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsInSearch))
            {
                var isInSearch = ViewModel.IsInSearch;
                UpdateViewState(true);
                InvokeAfterAnimation(() => {
                    if (!isInSearch)
                    {
                        // keeping searchbar visible after search ended
                        VenuesAndShowsTableView.SetActualContentOffset(0, false);
                    }
                });
            }
        }

        protected override void OnViewModelRefreshed(bool hasData, bool pullToRefresh)
        {
            base.OnViewModelRefreshed(hasData, pullToRefresh);

            UpdateTableViewInset();

            if (pullToRefresh)
            {
                _scrollToHideManager.Reset();
            }
        }

        protected override void ScrollViewToTop(bool animated)
        {
            var tableSource = VenuesAndShowsTableView.Source as HiddenHeaderTableSource<Venue>;
            if (tableSource != null)
            {
                tableSource.ScrollOutHeader(animated);
            }
            else
            {
                base.ScrollViewToTop(animated);
            }
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
            _dayButton = ButtonBarUtil.Create(SemiTransparentType.Light);
            _dayButton.LineBreakMode = UILineBreakMode.WordWrap;
            _dayButton.TouchUpInside += OnDayButtonClicked;

            _dayButtonItem = new UIBarButtonItem();
            _dayButtonItem.CustomView = _dayButton;

            navBarItems.Add(_dayButtonItem);

            // Mode (List, Map, Combined) Button
            _modeButton = ButtonBarUtil.Create(
                ThemeIcons.List, 
                ThemeIcons.ListLandscape, 
                SemiTransparentType.Light);
            _modeButton.TouchUpInside += OnModeButtonClicked;

            _modeButtonItem = new UIBarButtonItem();
            _modeButtonItem.CustomView = _modeButton;

            navBarItems.Add(_modeButtonItem);
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

        private void InvokeAfterAnimation(Action action)
        {
            var animationFinished = default(EventHandler);
            animationFinished = (s, e) => {
                AnimationFinished -= animationFinished;
                action();
            };

            if (_isAnimating)
            {
                AnimationFinished += animationFinished;
            }
            else
            {
                action();
            }
        }

        private void InitializeStyle()
        {
            MapFullscreenButton.SemiTransparentType = SemiTransparentType.Light;
            MapFullscreenButton.UpdateState();

            VenuesMapView.TintColor = ThemeColors.Metadata;
            VenuesMapView.FixLegalLabel();

            SearchBar.Placeholder = Localization.Search;
            SearchBar.TintColor = ThemeColors.ContentLightText;
            SearchBar.BarTintColor = null;
            SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            SearchBar.SetPassiveStyle();
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

        private void InitializeSearch()
        {
            var searchBarDelegate = new OrgEventSearchBarDelegate(SearchBar, ViewModel);
            SearchBar.Delegate = searchBarDelegate;
            
            var searchTableSource = 
                new OrgEventTableSource(SearchTableView, ViewModel);

            this.CreateBinding(searchTableSource)
                .For(ts => ts.ItemsSource)
                .To<OrgEventViewModel>(vm => vm.SearchListItems)
                .WithConversion(new OrgEventTableSourceConverter(), ViewModel)
                .Apply();

            SearchTableView.Source = searchTableSource;
        }

        private void EndSearch()
        {
            if (ViewModel.EndSearchCommand.CanExecute(null))
            {
                ViewModel.EndSearchCommand.Execute(null);
            }
        }

        private void UpdateSearchTableViewState()
        {
            SearchTableView.BackgroundColor = ViewModel.SearchListItems != null
                ? ThemeColors.ContentLightBackground 
                : ThemeColors.ContentDarkBackground.ColorWithAlpha(0.3f);
        }

        private void UpdateFavoritesUnavailableState()
        {
            if (ViewModel.Mode == OrgEventViewMode.List &&
                ViewModel.ShowOnlyFavorites &&
                ViewModel.ListItems != null &&
                ViewModel.ListItems.Length > 0 &&
                ViewModel.ListItems[0].Shows.Length == 0)
            {
                UpdateMessageState(Localization.FavoritesUnavailable, true);
            }
            else
            {
                UpdateMessageState(Localization.FavoritesUnavailable, false);
            }
        }

        private void InitializeGestures()
        {
            _searchTableTapGesture = new UITapGestureRecognizer(() =>
                {
                    if (ViewModel.SearchListItems == null)
                    {
                        EndSearch();
                    }
                });

            SearchTableView.AddGestureRecognizer(_searchTableTapGesture);
        }

        private void DisposeGestures()
        {
            if (_searchTableTapGesture != null)
            {
                SearchTableView.RemoveGestureRecognizer(_searchTableTapGesture);
                _searchTableTapGesture.Dispose();
                _searchTableTapGesture = null;
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
            EndSearch();

            var annotation = GetAnnotationByVenue(ViewModel.SelectedVenueOnMap);

            var shiftedCoord = annotation.Coordinate;
            shiftedCoord.Latitude += 0.0008f;

            VenuesMapView.SetRegion(
                MapUtil.CoordinateRegionForCoordinates(shiftedCoord), true);
        }

        private void OnScrollToVenue(object sender, MvxValueEventArgs<Venue> e)
        {
            InvokeAfterAnimation(() => ScrollToVenue(e.Value));
        }

        private void ScrollToVenue(Venue venue)
        {
            var tableSource = VenuesAndShowsTableView.WeakDelegate as OrgEventTableSource;
            if (tableSource != null)
            {
                var index = tableSource.GetItemIndex(venue);
                if (index != null)
                {
                    VenuesAndShowsTableView.ScrollToRow(
                        index, 
                        UITableViewScrollPosition.Top,
                        true);
                }
            }
        }

        private void OnFavoritesUpdated(object sender, EventArgs e)
        {
            // if favories updated in venue view
            if (View.Window == null)
            {
                _favoritesInvalidated = true;
            }
        }

        private void UpdateFavories()
        {
            if (_favoritesInvalidated)
            {
                _favoritesInvalidated = false;

                var tableSource = VenuesAndShowsTableView.WeakDelegate as OrgEventTableSource;
                if (tableSource != null)
                {
                    tableSource.ReloadTableData(false);
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
                _listSettingsView.ShowOnlyFavoritesCommand = ViewModel.ShowOnlyFavoritesCommand;
                _listSettingsView.ShowOnlyFavotires = ViewModel.ShowOnlyFavorites;
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
                    ListSettingsContainer.Hidden = true;
                    _scrollToHideManager.IsActive = false;
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
                    ListSettingsContainer.Hidden = true;
                    _scrollToHideManager.IsActive = false;
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
                    var listSettingsHidden = !HasData || IsInSearch;
                    ListSettingsContainer.SetHidden(listSettingsHidden, animated && !listSettingsHidden);
                    _scrollToHideManager.IsActive = !IsInSearch && HasData;
                    break;
            }

            UpdateNavBarState(animated);
            UpdateNavBarItemsState();
            UpdateDayButtonState();
            UpdateTableViewInset();
            UpdateViewConstraints(animated);
            UpdateSearchTableViewState();
            UpdateFavoritesUnavailableState();

            SearchBar.Hidden = !HasData;
            SearchTableView.SetHidden(!IsInSearch, animated);
            SearchTableView.ScrollsToTop = IsInSearch;
            VenuesAndShowsTableView.ScrollsToTop = !IsInSearch;
            VenuesAndShowsTableView.ScrollEnabled = !IsInSearch;
            View.BackgroundColor = IsInSearch 
                ? ThemeColors.PanelBackground 
                : ThemeColors.ContentLightBackground;

            SetNeedStatusBarUpdate(animated);
        }

        private void UpdateNavBarState(bool animated)
        {
            if (IsNavBarTransparent)
            {
                SetNavBarTransparent(SemiTransparentType.Light, animated);
            }
            else
            {
                SetNavBarTransparent(SemiTransparentType.None, animated);
            }
        }

        private void UpdateNavBarItemsState()
        {
            if (!HasData) return;

            var items = NavigationItem.RightBarButtonItems.ToList();
            if (!ViewModel.SwitchModeCommand.CanExecute(null))
            {
                if (items.IndexOf(_modeButtonItem) >= 0)
                {
                    items.Remove(_modeButtonItem);
                    NavigationItem.SetRightBarButtonItems(items.ToArray(), false);
                }
            }
            else
            {
                if (items.IndexOf(_modeButtonItem) < 0)
                {
                    var dayIndex = items.IndexOf(_dayButtonItem);
                    items.Insert(dayIndex, _modeButtonItem);
                    NavigationItem.SetRightBarButtonItems(items.ToArray(), false);
                }
            }
        }

        private void UpdateDayButtonState()
        {
            var dateString = ViewModel.OrgEvent.GetOrgEventDateString(
                NavBarManager.Instance.SemiTransparentType == SemiTransparentType.None
                    ? ThemeColors.ContentDarkText
                    : ThemeColors.ContentLightText,
                ViewModel.CurrentDay,
                ScreenUtil.IsVerticalOrientation);

            _dayButton.SetAttributedTitle(dateString, UIControlState.Normal);
            _dayButton.Hidden = dateString.Length <= 0;
            _dayButton.Enabled = ViewModel.SetCurrentDayCommand.CanExecute(null);

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
            if (HasData && !IsInSearch && ViewModel.Mode == OrgEventViewMode.List)
            {
                var topInset = NavBarManager.NavBarHeight + 
                    (!HasData ? 0 : ListSettingsView.DefaultHeight);

                VenuesAndShowsTableView.ContentInset = new UIEdgeInsets(topInset, 0, 0, 0);
                VenuesAndShowsTableView.ScrollIndicatorInsets = VenuesAndShowsTableView.ContentInset;
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