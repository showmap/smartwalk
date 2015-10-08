using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Resources;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgEventViewModel : RefreshableViewModel, IShareableViewModel, IFavoritesAware
    {
        private readonly IEnvironmentService _environmentService;
        private readonly ISmartWalkApiService _apiService;
        private readonly IConfiguration _configuration;
        private readonly IAnalyticsService _analyticsService;
        private readonly ICalendarService _calendarService;
        private readonly IExceptionPolicyService _exceptionPolicy;

        private OrgEvent _allDaysOrgEvent;
        private OrgEventViewMode _mode = OrgEventViewMode.Combo;
        private MapType _currentMapType = MapType.Standard;
        private OrgEvent _orgEvent;
        private int? _currentDay;
        private int _daysCount;
        private Show _expandedShow;
        private Venue _selectedVenueOnMap;
        private Parameters _parameters;
        private SortBy _sortBy = SortBy.Name;
        private Venue[] _searchResults;
        private Dictionary<SearchKey, string> _searchableTexts;
        private Venue[] _allShows;
        private Venue[] _allSearchShows;
        private OrgEventViewMode? _beforeSearchMode;
        private bool _showOnlyFavorites;
        private bool _isInSearch;
        private CalendarEvent _currentCalendarEvent;

        private MvxCommand _createEventCommand;
        private MvxCommand _saveEventCommand;
        private MvxCommand _cancelEventCommand;
        private MvxCommand _beginSearchCommand;
        private MvxCommand _endSearchCommand;
        private MvxCommand<string> _searchCommand;
        private MvxCommand<Show> _expandCollapseShowCommand;
        private MvxCommand<OrgEventViewMode?> _switchModeCommand;
        private MvxCommand<int?> _setCurrentDayCommand;
        private MvxCommand _switchMapFullscreenCommand;
        private MvxCommand _navigateOrgCommand;
        private MvxCommand _navigateOrgEventInfoCommand;
        private MvxCommand<Venue> _navigateVenueCommand;
        private MvxCommand<Venue> _selectVenueOnMapCommand;
        private MvxCommand<Venue> _navigateVenueOnMapCommand;
        private MvxCommand<SortBy> _sortByCommand;
        private MvxCommand _switchMapTypeCommand;
        private MvxCommand _copyLinkCommand;
        private MvxCommand _shareCommand;
        private MvxCommand<bool> _showOnlyFavoritesCommand;

        public OrgEventViewModel(
            IEnvironmentService environmentService,
            ISmartWalkApiService apiService,
            IConfiguration configuration,
            IAnalyticsService analyticsService,
            IExceptionPolicyService exceptionPolicy,
            ICalendarService calendarService,
            IPostponeService postponeService,
            IFavoritesService favoritesService) 
            : base(environmentService.Reachability, analyticsService, postponeService)
        {
            _environmentService = environmentService;
            _apiService = apiService;
            _configuration = configuration;
            _analyticsService = analyticsService;
            _calendarService = calendarService;
            _exceptionPolicy = exceptionPolicy;

            FavoritesManager = new FavoritesShowManager(favoritesService, analyticsService);
            FavoritesManager.FavoritesUpdated += (sender, e) => ResetAllShows();
        }

        public event EventHandler<MvxValueEventArgs<string>> Share;
        public event EventHandler<MvxValueEventArgs<Venue>> ZoomToVenue;
        public event EventHandler<MvxValueEventArgs<Venue>> ScrollToVenue;
        public event EventHandler<ValueChangedEventArgs<OrgEventViewMode>> ModeChanged;

        public override string Title
        {
            get { return OrgEvent != null ? OrgEvent.Title : null; }
        }

        public FavoritesShowManager FavoritesManager { get; private set; }

        public OrgEventViewMode Mode
        {
            get { return _mode; }
            private set
            {
                if (_mode != value)
                {
                    var previousMode = _mode;
                    _mode = value;
                    ExpandedShow = null;

                    if (ModeChanged != null)
                    {
                        ModeChanged(this, 
                            new ValueChangedEventArgs<OrgEventViewMode>(previousMode, _mode));
                    }

                    RaisePropertyChanged(() => Mode);
                    RaisePropertyChanged(() => ListItems);
                    RaisePropertyChanged(() => SearchListItems);
                }
            }
        }

        public MapType CurrentMapType
        {
            get { return _currentMapType; }
            private set
            {
                if (_currentMapType != value)
                {
                    _currentMapType = value;
                    RaisePropertyChanged(() => CurrentMapType);
                }
            }
        }

        public OrgEvent OrgEvent
        {
            get { return _orgEvent; }
            private set
            {
                if (!Equals(_orgEvent, value))
                {
                    _orgEvent = value;
                    _searchableTexts = null;
                    ResetAllShows();
                    RaisePropertyChanged(() => OrgEvent);
                    RaisePropertyChanged(() => ListItems);
                    RaisePropertyChanged(() => SearchListItems);
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        public Venue[] ListItems
        {
            get 
            { 
                return IsGroupedByLocation 
                    ? (OrgEvent != null ? OrgEvent.Venues : null)
                    : AllShows; 
            }
        }

        public Venue[] SearchResults
        {
            get { return _searchResults; }
            private set
            {
                if (!_searchResults.EnumerableEquals(value))
                {
                    _searchResults = value;
                    _allSearchShows = null;
                    RaisePropertyChanged(() => SearchResults);
                    RaisePropertyChanged(() => SearchListItems);
                }
            }
        }

        public Venue[] SearchListItems
        {
            get { return IsGroupedByLocation ? SearchResults : AllSearchShows; }
        }

        public int? CurrentDay
        {
            get { return _currentDay; }
            private set
            {
                if (_currentDay != value)
                {
                    _currentDay = value;
                    RaisePropertyChanged(() => CurrentDay);
                    OrgEvent = GetOrgEventByDay(_allDaysOrgEvent);
                }
            }
        }

        public int DaysCount
        {
            get { return _daysCount; }
            private set
            {
                if (_daysCount != value)
                {
                    _daysCount = value;
                    RaisePropertyChanged(() => DaysCount);
                    RaisePropertyChanged(() => IsMultiday);
                }
            }
        }

        public bool IsMultiday
        {
            get { return DaysCount > 1; }
        }

        public Show ExpandedShow
        {
            get { return _expandedShow; }
            private set
            {
                if (!Equals(_expandedShow, value))
                {
                    _expandedShow = value;
                    RaisePropertyChanged(() => ExpandedShow);
                }
            }
        }

        public Venue SelectedVenueOnMap
        {
            get { return _selectedVenueOnMap; }
            private set
            {
                if (!Equals(_selectedVenueOnMap, value))
                {
                    _selectedVenueOnMap = value;
                    RaisePropertyChanged(() => SelectedVenueOnMap);
                }
            }
        }

        public bool IsGroupedByLocation
        {
            get
            {
                return Mode != OrgEventViewMode.List || 
                    (IsInSearch && _beforeSearchMode.HasValue && 
                        _beforeSearchMode.Value != OrgEventViewMode.List);
            }
        }

        public SortBy SortBy
        {
            get { return _sortBy; }
            private set
            {
                if (_sortBy != value)
                {
                    _sortBy = value;
                    ExpandedShow = null;
                    ResetAllShows();
                    RaisePropertyChanged(() => SortBy);
                    RaisePropertyChanged(() => ListItems);
                    RaisePropertyChanged(() => SearchListItems);
                }
            }
        }

        public bool ShowOnlyFavorites
        {
            get { return _showOnlyFavorites; }
            private set
            {
                if (_showOnlyFavorites != value)
                {
                    _showOnlyFavorites = value;
                    ExpandedShow = null;
                    ResetAllShows();
                    RaisePropertyChanged(() => ShowOnlyFavorites);
                    RaisePropertyChanged(() => ListItems);
                    RaisePropertyChanged(() => SearchListItems);
                }
            }
        }

        public bool IsInSearch
        {
            get { return _isInSearch; }
            private set 
            { 
                if (_isInSearch != value)
                {
                    _isInSearch = value;
                    RaisePropertyChanged(() => IsInSearch);
                    RaisePropertyChanged(() => ListItems);
                }
            }
        }

        public CalendarEvent CurrentCalendarEvent
        {
            get
            {
                return _currentCalendarEvent;
            }
            private set
            {
                if (!Equals(_currentCalendarEvent, value))
                {
                    _currentCalendarEvent = value;
                    RaisePropertyChanged(() => CurrentCalendarEvent);
                }
            }
        }

        public ICommand CreateEventCommand
        {
            get
            {
                if (_createEventCommand == null)
                {
                    _createEventCommand = new MvxCommand(async () => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCreateEvent);

                            if (OrgEvent != null)
                            {
                                try
                                {
                                    CurrentCalendarEvent = 
                                        await _calendarService.CreateNewEvent(OrgEvent);
                                }
                                catch (Exception ex)
                                {
                                    _exceptionPolicy.Trace(ex);
                                }
                            }
                            else 
                            {
                                _environmentService.Alert(
                                    Localization.OffLineMode, 
                                    Localization.CantCompleteActionOffline);
                            }
                        },
                        () => 
                        OrgEvent != null &&
                        OrgEvent.Id != 0);
                }

                return _createEventCommand;
            }
        }

        public ICommand SaveEventCommand
        {
            get
            {
                if (_saveEventCommand == null)
                {
                    _saveEventCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelSaveEvent);

                            try
                            {
                                _calendarService.SaveEvent(CurrentCalendarEvent);
                                CurrentCalendarEvent = null;
                            }
                            catch (Exception ex)
                            {
                                _exceptionPolicy.Trace(ex);
                            }
                        },
                        () => CurrentCalendarEvent != null);
                }

                return _saveEventCommand;
            }
        }

        public ICommand CancelEventCommand
        {
            get
            {
                if (_cancelEventCommand == null)
                {
                    _cancelEventCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCancelEvent);

                            CurrentCalendarEvent = null;
                        },
                        () => CurrentCalendarEvent != null);
                }

                return _cancelEventCommand;
            }
        }

        public ICommand BeginSearchCommand
        {
            get
            {
                if (_beginSearchCommand == null)
                {
                    _beginSearchCommand = new MvxCommand(() => 
                        {
                            IsInSearch = true;

                            if (Mode != OrgEventViewMode.List)
                            {
                                _beforeSearchMode = Mode;
                                Mode = OrgEventViewMode.List;
                            }

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelBeginSearch);
                        },
                        () => !IsInSearch && SearchableTexts != null);
                }

                return _beginSearchCommand;
            }
        }

        public ICommand EndSearchCommand
        {
            get
            {
                if (_endSearchCommand == null)
                {
                    _endSearchCommand = new MvxCommand(() => 
                        {
                            if (_beforeSearchMode.HasValue)
                            {
                                Mode = _beforeSearchMode.Value;
                                _beforeSearchMode = null;
                            }

                            IsInSearch = false;
                            SearchResults = null;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelEndSearch);
                        },
                        () => IsInSearch);
                }

                return _endSearchCommand;
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new MvxCommand<string>(query => 
                        {
                            var searchResults = new List<Venue>();

                            if (!string.IsNullOrWhiteSpace(query))
                            {
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelSearchInEvent);

                                var matches = SearchableTexts
                                    .Where(kvp => kvp.Value != null && 
                                        kvp.Value.Contains(query.ToLower()))
                                    .ToArray();

                                foreach (var match in matches)
                                {
                                    // Analysis disable AccessToModifiedClosure
                                    var venue = searchResults
                                        .FirstOrDefault(v => Equals(v.Info, match.Key.Venue.Info));
                                    // Analysis restore AccessToModifiedClosure
                                    if (venue == null)
                                    {
                                        var shows = match.Key.Show != null 
                                            ? new [] { match.Key.Show } 
                                            : new Show[0];
                                        venue = new Venue(
                                            match.Key.Venue.Info,
                                            match.Key.Venue.Description) { 
                                            Shows = shows 
                                        };
                                        searchResults.Add(venue);
                                    }
                                    else
                                    {
                                        venue.Shows = venue.Shows.Union(new [] {match.Key.Show}).ToArray();
                                    }
                                }
                            }

                            SearchResults = searchResults.Count > 0 
                                ? searchResults.ToArray() 
                                : null;
                        },
                        query => IsInSearch && SearchableTexts != null);
                }

                return _searchCommand;
            }
        }

        public ICommand ExpandCollapseShowCommand
        {
            get
            {
                if (_expandCollapseShowCommand == null)
                {
                    _expandCollapseShowCommand = new MvxCommand<Show>(show => 
                        {
                            ExpandedShow = !Equals(ExpandedShow, show) ? show : null;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                ExpandedShow != null 
                                    ? Analytics.ActionLabelExpandVenueShow 
                                    : Analytics.ActionLabelCollapseVenueShow,
                                GetExpandedShowNumber());
                        },
                    venue => _parameters != null);
                }

                return _expandCollapseShowCommand;
            }
        }

        public ICommand SwitchModeCommand
        {
            get 
            {
                if (_switchModeCommand == null)
                {
                    _switchModeCommand = new MvxCommand<OrgEventViewMode?>(
                        mode => 
                        {
                            if (mode.HasValue)
                            {
                                Mode = mode.Value;
                            }
                            else
                            {
                                switch (Mode)
                                {
                                    case OrgEventViewMode.Combo:
                                        Mode = OrgEventViewMode.List;
                                        break;

                                    case OrgEventViewMode.List:
                                        Mode = OrgEventViewMode.Combo;
                                        break;

                                    case OrgEventViewMode.Map:
                                        Mode = OrgEventViewMode.List;
                                        break;
                                }
                            }

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Mode == OrgEventViewMode.List 
                                    ? Analytics.ActionLabelSwitchToList 
                                    : (Mode == OrgEventViewMode.Map
                                        ? Analytics.ActionLabelSwitchToMap
                                        : Analytics.ActionLabelSwitchToCombo));
                        },
                        mode => mode.HasValue && mode.Value != OrgEventViewMode.List || AnyShows);
                }

                return _switchModeCommand;
            }
        }

        public ICommand SwitchMapFullscreenCommand
        {
            get 
            {
                if (_switchMapFullscreenCommand == null)
                {
                    _switchMapFullscreenCommand = new MvxCommand(
                        () => 
                        {
                            switch (Mode)
                            {
                                case OrgEventViewMode.Combo:
                                    Mode = OrgEventViewMode.Map;
                                    break;

                                case OrgEventViewMode.Map:
                                    Mode = OrgEventViewMode.Combo;
                                    break;

                                case OrgEventViewMode.List:
                                    Mode = OrgEventViewMode.Map;
                                    break;
                            }

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Mode == OrgEventViewMode.List 
                                ? Analytics.ActionLabelSwitchToList 
                                : (Mode == OrgEventViewMode.Map
                                    ? Analytics.ActionLabelSwitchToMap
                                    : Analytics.ActionLabelSwitchToCombo));
                        });
                }

                return _switchMapFullscreenCommand;
            }
        }

        public ICommand SwitchMapTypeCommand
        {
            get
            {
                if (_switchMapTypeCommand == null)
                {
                    _switchMapTypeCommand = new MvxCommand(
                        () =>
                        {
                            CurrentMapType = CurrentMapType.GetNextMapType();

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                CurrentMapType == MapType.Standard 
                                ? Analytics.ActionLabelSwitchMapToStandard
                                : (CurrentMapType == MapType.Satellite
                                    ? Analytics.ActionLabelSwitchMapToSatellite
                                    : Analytics.ActionLabelSwitchMapToHybrid));
                        },
                        () => 
                            OrgEvent != null &&
                            (Mode == OrgEventViewMode.Combo ||
                                Mode == OrgEventViewMode.Map));
                }

                return _switchMapTypeCommand;
            }
        }

        public ICommand SetCurrentDayCommand
        {
            get
            {
                if (_setCurrentDayCommand == null)
                {
                    _setCurrentDayCommand = new MvxCommand<int?>(
                        day => 
                            {
                                if (day.HasValue)
                                {
                                    CurrentDay = day;
                                }
                                else
                                {
                                    if (CurrentDay.HasValue && CurrentDay.Value < DaysCount)
                                    {
                                        CurrentDay++;
                                    }
                                    else if (!CurrentDay.HasValue)
                                    {
                                        CurrentDay = 1;
                                    }
                                    else 
                                    {
                                        CurrentDay = null;
                                    }
                                }
                            },
                        day => 
                            OrgEvent != null &&
                            IsMultiday &&
                            (!day.HasValue ||
                                day.Value < (DaysCount + 1)));
                }

                return _setCurrentDayCommand;
            }
        }

        public ICommand NavigateOrgCommand
        {
            get
            {
                if (_navigateOrgCommand == null)
                {
                    _navigateOrgCommand = new MvxCommand(
                        () => ShowViewModel<OrgViewModel>(
                            new OrgViewModel.Parameters {
                                OrgId = OrgEvent.OrgId,
                                Location = _parameters.Location
                            }),
                        () => 
                            _parameters != null &&
                            _parameters.Current &&
                            OrgEvent != null &&
                            OrgEvent.OrgId != 0);
                }

                return _navigateOrgCommand;
            }
        }

        public ICommand NavigateOrgEventInfoCommand
        {
            get
            {
                if (_navigateOrgEventInfoCommand == null)
                {
                    _navigateOrgEventInfoCommand = new MvxCommand(
                        () => ShowViewModel<OrgEventInfoViewModel>(
                            new OrgEventInfoViewModel.Parameters {
                                EventId = OrgEvent.Id,
                                Location = _parameters.Location
                            }),
                        () => 
                            _parameters != null &&
                            OrgEvent != null &&
                            OrgEvent.Id != 0 &&
                            !string.IsNullOrWhiteSpace(OrgEvent.Description));
                }

                return _navigateOrgEventInfoCommand;
            }
        }

        public ICommand NavigateVenueCommand
        {
            get
            {
                if (_navigateVenueCommand == null)
                {
                    _navigateVenueCommand = new MvxCommand<Venue>(
                        venue => ShowViewModel<VenueViewModel>(
                            new VenueViewModel.Parameters {
                                VenueId = venue.Info.Id,
                                EventId = _parameters.EventId,
                                Location = _parameters.Location
                            }),
                        venue => 
                            venue != null && 
                            _parameters != null &&
                            _parameters.EventId != 0);
                }

                return _navigateVenueCommand;
            }
        }

        public ICommand SelectVenueOnMapCommand
        {
            get
            {
                if (_selectVenueOnMapCommand == null)
                {
                    _selectVenueOnMapCommand = new MvxCommand<Venue>(
                        venue => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                venue != null
                                    ? Analytics.ActionLabelSelectVenueOnMap
                                    : Analytics.ActionLabelDeselectVenueOnMap,
                                venue != null ? venue.Info.Id : 0);

                            SelectedVenueOnMap = venue;

                            if (ScrollToVenue != null && venue != null)
                            {
                                ScrollToVenue(this, new MvxValueEventArgs<Venue>(venue));
                            }
                        });
                }

                return _selectVenueOnMapCommand;
            }
        }

        public ICommand NavigateVenueOnMapCommand
        {
            get
            {
                if (_navigateVenueOnMapCommand == null)
                {
                    _navigateVenueOnMapCommand = new MvxCommand<Venue>(
                        venue =>
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelNavigateVenueOnMap,
                                venue.Info.Id);

                            var previousMode = Mode;
                            Mode = OrgEventViewMode.Combo;
                            
                            SelectedVenueOnMap = venue;

                            if (ZoomToVenue != null && venue != null)
                            {
                                ZoomToVenue(this, new MvxValueEventArgs<Venue>(venue));
                            }

                            // if navigation is from show cell location
                            if (ScrollToVenue != null && venue != null && previousMode == OrgEventViewMode.List)
                            {
                                ScrollToVenue(this, new MvxValueEventArgs<Venue>(venue));
                            }
                        },
                        venue => venue != null);
                }

                return _navigateVenueOnMapCommand;
            }
        }

        public ICommand SortByCommand
        {
            get
            {
                if (_sortByCommand == null)
                {
                    _sortByCommand = new MvxCommand<SortBy>(
                        sortBy => 
                        { 
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                sortBy == SortBy.Time
                                    ? Analytics.ActionLabelSortShowsByTime
                                    : Analytics.ActionLabelSortShowsByTitle);
                            
                            SortBy = sortBy;
                        });
                }

                return _sortByCommand;
            }
        }

        public ICommand ShowOnlyFavoritesCommand
        {
            get
            {
                if (_showOnlyFavoritesCommand == null)
                {
                    _showOnlyFavoritesCommand = new MvxCommand<bool>(
                        showOnly =>
                        { 
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                showOnly
                                    ? Analytics.ActionLabelOnlyFavoriteOn
                                    : Analytics.ActionLabelOnlyFavoriteOff);

                            ShowOnlyFavorites = showOnly;
                        },
                        showOnly => showOnly != ShowOnlyFavorites);
                }

                return _showOnlyFavoritesCommand;
            }
        }

        public ICommand CopyLinkCommand
        {
            get
            {
                if (_copyLinkCommand == null)
                {
                    _copyLinkCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelCopyLink);

                            var eventUrl = _configuration.GetEventUrl(_parameters.EventId);
                            _environmentService.Copy(eventUrl);
                        },
                        () => 
                            _parameters != null &&
                            _parameters.EventId != 0);
                }

                return _copyLinkCommand;
            }
        }

        public ICommand ShareCommand
        {
            get
            {
                if (_shareCommand == null)
                {
                    _shareCommand = new MvxCommand(() => 
                        {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelShare);

                            var eventUrl = _configuration.GetEventUrl(_parameters.EventId);
                            if (eventUrl != null && Share != null)
                            {
                                Share(this, new MvxValueEventArgs<string>(eventUrl));
                            }
                        },
                        () => 
                            _parameters != null &&
                            _parameters.EventId != 0);
                }

                return _shareCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        private Venue[] AllShows
        {
            get
            {
                if (_allShows == null &&
                    OrgEvent != null &&
                    OrgEvent.Venues != null)
                {
                    var shows = OrgEvent.Venues
                        .SelectMany(v => v.Shows ?? new Show[] { })
                        .ToArray();
                    _allShows = GetAllShows(shows);
                }

                return _allShows;
            }
        }

        private Venue[] AllSearchShows
        {
            get
            {
                if (_allSearchShows == null &&
                    SearchResults != null)
                {
                    var shows = SearchResults
                        .SelectMany(v => v.Shows ?? new Show[] { })
                        .ToArray();
                    _allSearchShows = GetAllShows(shows);
                }

                return _allSearchShows;
            }
        }

        private Dictionary<SearchKey, string> SearchableTexts
        {
            get
            {
                if (_searchableTexts == null && 
                    OrgEvent != null &&
                    OrgEvent.Venues != null)
                {
                    _searchableTexts = new Dictionary<SearchKey, string>();

                    foreach (var venue in OrgEvent.Venues)
                    {
                        var text = venue.GetSearchableText();
                        _searchableTexts[new SearchKey(venue)] = 
                            text != null ? text.ToLower() : null;

                        if (venue.Shows != null)
                        {
                            foreach (var show in venue.Shows)
                            {
                                text = show.GetSearchableText();
                                _searchableTexts[new SearchKey(venue, show)] = 
                                    text != null ? text.ToLower() : null;
                            }
                        }
                    }
                }

                return _searchableTexts;
            }
        }

        private bool AnyShows
        {
            get
            { 
                var result = 
                    _orgEvent != null &&
                    _orgEvent.Venues != null &&
                    _orgEvent.Venues
                    .Any(v => 
                        v.Shows != null &&
                        v.Shows.Length > 0);
                return result; 
            }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateData(UpdateOrgEvent);
        }

        protected override void Refresh(DataSource source)
        {
            UpdateOrgEvent(source).ContinueWithThrow();
        }

        private async Task<DataSource> UpdateOrgEvent(DataSource source, bool showProgress = true)
        {
            if (_parameters != null)
            {
                if (showProgress) IsLoading = true;

                var orgEvent = default(OrgEvent);

                try 
                {
                    var result = await _apiService.GetOrgEvent(
                        _parameters.EventId, 
                        source);
                    if (result != null)
                    {
                        orgEvent = result.Data;
                        source = result.Source;
                    }
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                if (showProgress) IsLoading = false;

                if (orgEvent != null)
                {
                    UpdateDaysState(orgEvent);
                    _allDaysOrgEvent = orgEvent;

                    if (!CurrentDay.HasValue)
                    {
                        OrgEvent = orgEvent;
                    }
                    else
                    {
                        OrgEvent = GetOrgEventByDay(orgEvent);
                    }
                }

                if (showProgress)
                {
                    RaiseRefreshCompleted(OrgEvent != null);
                }
            }
            else
            {
                _allDaysOrgEvent = null;
                OrgEvent = null;
            }

            return source;
        }

        private void UpdateDaysState(OrgEvent orgEvent)
        {
            if (orgEvent == null
                ||
                (!orgEvent.StartTime.HasValue ||
                    !orgEvent.EndTime.HasValue)
                ||
                (orgEvent.StartTime.Value.Date ==
                    orgEvent.EndTime.Value.Date))
            {
                DaysCount = 0;
                CurrentDay = null;
            }
            else
            {
                var startDate = orgEvent.StartTime.Value;
                var endDate = orgEvent.EndTime.Value;

                DaysCount = DateTimeExtensions.DaysCount(
                    orgEvent.StartTime, 
                    orgEvent.EndTime);

                if (startDate <= DateTime.Now.Date &&
                    DateTime.Now.Date <= endDate)
                {
                    CurrentDay = (DateTime.Now.Date - startDate).Days + 1;
                }
                else
                {
                    CurrentDay = null; // Maybe persist day on refresh
                }
            }
        }

        private OrgEvent GetOrgEventByDay(OrgEvent orgEvent)
        {
            if (orgEvent == null || 
                !orgEvent.StartTime.HasValue || 
                !CurrentDay.HasValue) return orgEvent;

            var firstDay = orgEvent.StartTime.Value.Date;
            var day = firstDay.AddDays(CurrentDay.Value - 1);

            var venues = 
                orgEvent.Venues
                .Select(
                    v => 
                        new Venue(v.Info, v.Description)
                        { 
                            Shows = GetShowsByDay(v.Shows, day, orgEvent.GetOrgEventRange())
                        })
                    // taking venues without any shows or the ones that has shows for current day
                    .Where(v => v.Shows == null || v.Shows.Length > 0)
                    .ToArray();


            var orgEventByDay = 
                new OrgEvent(
                    orgEvent.EventMetadata,
                    orgEvent.Host,
                    venues);
            return orgEventByDay;
        }

        private static Show[] GetShowsByDay(
            Show[] shows, 
            DateTime day, 
            Tuple<DateTime?, DateTime?> range)
        {
            if (shows == null) return null;

            var result = 
                shows
                    .Where(
                        s => s.StartTime.IsTimeThisDay(day, range))
                    .ToArray();

            return result;
        }

        private int GetExpandedShowNumber()
        {
            if (OrgEvent != null && ExpandedShow != null)
            {
                var venue = OrgEvent.Venues.GetVenueByShow(ExpandedShow);
                var venueNumber = Array.IndexOf(OrgEvent.Venues, venue);
                var showNumber = Array.IndexOf(venue.Shows, ExpandedShow);

                return venueNumber * 100 + showNumber;
            }

            return 0;
        }

        private Venue[] GetAllShows(Show[] shows)
        {
            var result = 
                new [] {
                    new Venue(new Entity(), null) 
                    {
                        Shows = shows
                            .Where(s => !ShowOnlyFavorites || FavoritesManager.IsShowFavorite(OrgEvent.Id, s))
                            .OrderBy(s => s, new ShowComparer(SortBy))
                            .ToArray()
                    }
                };
            return result;
        }

        private void ResetAllShows()
        {
            _allShows = null;
            _allSearchShows = null;
        }

        public class Parameters : ParametersBase
        {
            public int EventId { get; set; }

            [IgnoreDataMember]
            public bool Current { get; set; }

            public override bool Equals(object obj)
            {
                var parameters = obj as Parameters;
                if (parameters != null)
                {
                    return Equals(Location, parameters.Location) && 
                        EventId == parameters.EventId &&
                        Current == parameters.Current;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Initial
                    .CombineHashCodeOrDefault(Location)
                    .CombineHashCode(EventId)
                    .CombineHashCode(Current);
            }
        }

        private class SearchKey
        {
            public SearchKey(Venue venue)
            {
                Venue = venue;
            }

            public SearchKey(Venue venue, Show show) : this(venue)
            {
                Show = show;
            }

            public Venue Venue { get; private set; }
            public Show Show { get; private set; }
        }
    }

    public enum OrgEventViewMode
    {
        Combo,
        List,
        Map
    }

    public enum SortBy
    {
        Time = 0,
        Name = 1
    }
}