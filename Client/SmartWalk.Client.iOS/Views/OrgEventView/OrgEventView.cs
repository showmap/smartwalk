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
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventView : ListViewBase
    {
        private OrgEventHeaderView _headerView;
        private UISearchDisplayController _searchDisplayController;
        private EKEventEditViewController _editCalEventController;
        private ListSettingsView _listSettingsView;
        private bool _isMapViewInitialized;
        private Show _previousExpandedShow;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.ZoomSelectedVenue += OnZoomSelectedVenue;
            ViewModel.ScrollSelectedVenue += OnScrollSelectedVenue;

            InitializeStyle();

            UpdateTableViewInset();
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

            if (_listSettingsView != null)
            {
                SetDialogViewFullscreenFrame(_listSettingsView);
            }
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel.ZoomSelectedVenue -= OnZoomSelectedVenue;
                ViewModel.ScrollSelectedVenue -= OnScrollSelectedVenue;

                DisposeToolBar();
                DisposeTableHeader();
                DisposeSearchDisplayController();
                DisposeMapView();
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

            // hiding ListOptions on rotation
            if (ViewModel.ShowHideListOptionsCommand.CanExecute(false))
            {
                ViewModel.ShowHideListOptionsCommand.Execute(false);
            }
        }

        public override void UpdateViewConstraints()
        {
            base.UpdateViewConstraints();

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

                    MapToTableConstraint.Constant = -VenuesAndShowsTableView.ContentInset.Top;
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

                    MapToTableConstraint.Constant = 0;
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

                    MapToTableConstraint.Constant = 0;
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
                UpdateTableViewInset();
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
                UpdateTableHeaderState(HasData);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsListOptionsShown))
            {
                ShowHideListSettingsView(ViewModel.IsListOptionsShown);
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsMultiday) ||
                propertyName == ViewModel.GetPropertyName(vm => vm.CurrentDay))
            {
                UpdateDayButtonState();
            }
        }

        protected override void OnViewModelRefreshed(bool hasData)
        {
            base.OnViewModelRefreshed(hasData);

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
            if (ViewModel.CreateEventCommand.CanExecute(null))
            {
                titles.Add(Localization.SaveToCalendar);
            }

            if (ViewModel.NavigateOrgEventInfoCommand.CanExecute(null))
            {
                titles.Add(Localization.ShowEventInfo);
            }

            if (ViewModel.NavigateOrgCommand.CanExecute(null))
            {
                titles.Add(Localization.ShowOrganizerInfo);
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

        protected override void OnInitializeNavBarItems(List<UIBarButtonItem> navBarItems)
        {
            base.OnInitializeNavBarItems(navBarItems);

            // Day Button
            _dayButton = ButtonBarUtil.Create(true);
            _dayButton.LineBreakMode = UILineBreakMode.WordWrap;
            _dayButton.TouchUpInside += OnDayButtonClicked;

            var dayButtonItem = new UIBarButtonItem();
            dayButtonItem.CustomView = _dayButton;

            navBarItems.Add(dayButtonItem);

            // Mode (List, Map, Combined) Button
            _modeButton = ButtonBarUtil.Create(ThemeIcons.NavBarList, ThemeIcons.NavBarListLandscape, true);
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
            MapFullscreenButton.IsSemiTransparent = true;
            VenuesMapView.TintColor = Theme.MapTint;
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

            var searchTableSource = new OrgEventTableSource(ViewModel)
                {
                    IsSearchSource = true,
                    TableView = _searchDisplayController.SearchResultsTableView
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
            _editCalEventController = new OrgEventEditViewController();
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
                    View.Add(_listSettingsView, true);
                }
            }
            else if (_listSettingsView != null)
            {
                _listSettingsView.RemoveFromSuperview(true);
                DisposeListSettingsView();
            }

            var tableSoure = VenuesAndShowsTableView.WeakDataSource as HiddenHeaderTableSource<Venue>;
            if (tableSoure != null)
            {
                tableSoure.IsAutohidingEnabled = !isShown;
            }
        }

        private float GetListSettingsTopMargin()
        {
            var headerLocation = 
                View.ConvertPointFromView(
                    _headerView.Frame.Location, 
                    _headerView);
                
            var result = headerLocation.Y + _headerView.Frame.Height;
            return result;
        }

        private void InitializeListSettingsView()
        {
            _listSettingsView = ListSettingsView.Create();

            SetDialogViewFullscreenFrame(_listSettingsView);
            UpdateListSettingsView();

            _listSettingsView.IsGroupByLocation = ViewModel.IsGroupedByLocation;
            _listSettingsView.SortBy = ViewModel.SortBy;

            _listSettingsView.Initialize();

            _listSettingsView.GroupByLocationCommand = ViewModel.GroupByLocationCommand;
            _listSettingsView.SortByCommand = ViewModel.SortByCommand;
            _listSettingsView.CloseCommand = ViewModel.ShowHideListOptionsCommand;
        }

        private void UpdateListSettingsView()
        {
            _listSettingsView.MarginTop = GetListSettingsTopMargin();
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
            switch (CurrentMode)
            {
                case OrgEventViewMode.Combo:
                    _modeButton.Hidden = false;
                    _modeButton.VerticalIcon = ThemeIcons.NavBarList;
                    _modeButton.LandscapeIcon = ThemeIcons.NavBarListLandscape;
                    _modeButton.UpdateState();

                    MapFullscreenButton.VerticalIcon = ThemeIcons.Fullscreen;
                    MapFullscreenButton.UpdateState();

                    VenuesAndShowsTableView.Hidden = false;
                    MapPanel.Hidden = false;
                    MapFullscreenButton.SetHidden(false, animated);

                    DeactivateSearchController();
                    InitializeMapView();
                    break;

                case OrgEventViewMode.Map:
                    _modeButton.Hidden = false;
                    _modeButton.VerticalIcon = ThemeIcons.NavBarList;
                    _modeButton.LandscapeIcon = ThemeIcons.NavBarListLandscape;
                    _modeButton.UpdateState();

                    MapFullscreenButton.VerticalIcon = ThemeIcons.ExitFullscreen;
                    MapFullscreenButton.UpdateState();

                    VenuesAndShowsTableView.SetHidden(true, animated);
                    MapPanel.Hidden = false;
                    MapFullscreenButton.SetHidden(false, animated);

                    DeactivateSearchController();
                    InitializeMapView();
                    break;

                case OrgEventViewMode.List:
                    _modeButton.Hidden = !HasData;
                    _modeButton.VerticalIcon = ThemeIcons.NavBarMap;
                    _modeButton.LandscapeIcon = ThemeIcons.NavBarMapLandscape;
                    _modeButton.UpdateState();

                    if (HasData)
                    {
                        VenuesAndShowsTableView.Hidden = false;
                    }

                    MapPanel.SetHidden(true, animated);
                    MapFullscreenButton.SetHidden(true, animated);
                    break;
            }

            UpdateNavBarState(animated);
            UpdateViewConstraints(animated);
            SetNeedStatusBarUpdate(animated);

            // hiding ListOptions on switching to any state
            if (ViewModel.ShowHideListOptionsCommand.CanExecute(false))
            {
                ViewModel.ShowHideListOptionsCommand.Execute(false);
            }
        }

        private void UpdateNavBarState(bool animated)
        {
            if (CurrentMode == OrgEventViewMode.List && HasData)
            {
                SetNavBarTransparent(false, animated);
            }
            else
            {
                SetNavBarTransparent(true, animated);
            }
        }

        private void UpdateDayButtonState()
        {
            var dateString = GetOrgEventDate();
            if (dateString != null)
            {
                _dayButton.Hidden = false;
                _dayButton.SetAttributedTitle(GetOrgEventDate(), UIControlState.Normal);
            }
            else
            {
                _dayButton.Hidden = true;
            }

            if (ViewModel.IsMultiday)
            {
                _dayButton.SetBackgroundImage(Theme.BlackImage, UIControlState.Highlighted);

                if (ViewModel.CurrentDay == null)
                {
                    _dayButton.TitleEdgeInsets = new UIEdgeInsets(-3, 0.5f, 0, 0);
                }
                else
                {
                    _dayButton.TitleEdgeInsets = UIEdgeInsets.Zero;
                }
            }
            else
            {
                _dayButton.SetBackgroundImage(null, UIControlState.Highlighted);
                _dayButton.TitleEdgeInsets = UIEdgeInsets.Zero;
            }
        }

        private NSAttributedString GetOrgEventDate()
        {
            if (ViewModel.OrgEvent == null ||
                !ViewModel.OrgEvent.StartTime.HasValue) return null;

            var result = new NSMutableAttributedString();

            result.Append(
                new NSAttributedString(
                    string.Format("{0:MMM}{1}", 
                        ViewModel.OrgEvent.StartTime.Value,
                        Environment.NewLine).ToUpper(),
                    ScreenUtil.IsVerticalOrientation
                        ? Theme.OrgEventMonthFont
                        : Theme.OrgEventMonthLandscapeFont,
                    Theme.NavBarText,
                    null,
                    null,
                    new NSMutableParagraphStyle { 
                        Alignment = UITextAlignment.Center
                    }));

            if (ViewModel.IsMultiday && ViewModel.CurrentDay == null)
            {
                result.Append(
                    new NSAttributedString(
                        string.Format("{0}-{1}", 
                            ViewModel.OrgEvent.StartTime.Value.Day,
                            ViewModel.OrgEvent.EndTime.Value.Day),
                        ScreenUtil.IsVerticalOrientation
                            ? Theme.OrgEventTwoDaysFont
                            : Theme.OrgEventTwoDaysLandscapeFont,
                        Theme.NavBarText,
                        null,
                        null,
                        new NSMutableParagraphStyle { 
                            Alignment = UITextAlignment.Center,
                            LineHeightMultiple = 0.85f
                        }));
            }
            else
            {
                result.Append(
                    new NSAttributedString(
                        string.Format("{0}", 
                            ViewModel.OrgEvent.StartTime.Value
                                .AddDays((ViewModel.CurrentDay ?? 1) - 1).Day),
                        ScreenUtil.IsVerticalOrientation
                            ? Theme.OrgEventDayFont
                            : Theme.OrgEventDayLandscapeFont,
                        Theme.NavBarText,
                        null,
                        null,
                        new NSMutableParagraphStyle { 
                            Alignment = UITextAlignment.Center,
                            LineHeightMultiple = 0.85f
                        }));
            }

            return result;
        }

        private void UpdateTableViewOnShowExpanding(UITableView tableView)
        {
            var tableSoure = tableView.WeakDataSource as OrgEventTableSource;
            if (tableView.VisibleCells.Length == 0 ||
                tableSoure == null) return;

            foreach (var cell in tableView.VisibleCells.OfType<VenueShowCell>())
            {
                var isExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                cell.HeaderView = 
                    tableSoure.GetHeaderForShowCell(isExpanded, cell.DataContext);
                cell.SubHeaderView = 
                    tableSoure.GetSubHeaderForShowCell(isExpanded, cell.DataContext);
                cell.SetIsExpanded(isExpanded, true);
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

                var headerHeight = 
                    (tableSoure.GetIsCellHeaderVisibile() ? VenueHeaderView.DefaultHeight : 0) +
                    (tableSoure.GetIsCellSubHeaderVisibile() ? GroupHeaderView.DefaultHeight : 0);

                var previuosCellHeight = 
                    previousIndex != null &&
                    currentIndex != null &&
                    previousIndex.Section < currentIndex.Section
                        ? VenueShowCell.CalculateCellHeight(
                            tableView.Frame.Width, true, false, false,
                            _previousExpandedShow) + 25 // just a magic number, really
                        : 0;

                var delta = 
                    (ViewModel.ExpandedShow != null ? 1 : -1) *
                    headerHeight - previuosCellHeight;

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

        private void UpdateTableViewInset()
        {
            if (IsInSearch) return;

            if (HasData)
            {
                var previousOffset = VenuesAndShowsTableView.ContentOffset;
                var delta = VenuesAndShowsTableView.ContentInset.Top - NavBarManager.NavBarHeight;

                VenuesAndShowsTableView.ContentInset = 
                    new UIEdgeInsets(NavBarManager.NavBarHeight, 0, 0, 0);
                VenuesAndShowsTableView.ScrollIndicatorInsets =
                    VenuesAndShowsTableView.ContentInset;

                VenuesAndShowsTableView.ContentOffset =
                    new PointF(previousOffset.X, previousOffset.Y + delta);
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
}