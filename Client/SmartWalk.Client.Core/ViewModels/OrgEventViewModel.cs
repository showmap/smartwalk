using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgEventViewModel : RefreshableViewModel, 
        IFullscreenImageProvider, 
        IShareableViewModel
    {
        private readonly IClipboard _clipboard;
        private readonly ISmartWalkApiService _apiService;
        private readonly IConfiguration _configuration;
        private readonly IAnalyticsService _analyticsService;
        private readonly ICalendarService _calendarService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private OrgEvent _allDaysOrgEvent;
        private OrgEventViewMode _mode = OrgEventViewMode.Combo;
        private OrgEvent _orgEvent;
        private int? _currentDay;
        private int _daysCount;
        private Show _expandedShow;
        private Venue _selectedVenueOnMap;
        private string _currentFullscreenImage;
        private CalendarEvent _currentCalendarEvent;
        private Parameters _parameters;
        private bool _isGroupedByLocation = true;
        private bool _isListOptionsShown;
        private bool _isListOptionsAvailable;
        private SortBy _sortBy = SortBy.Time;
        private Venue[] _searchResults;
        private Dictionary<SearchKey, string> _searchableTexts;
        private Venue[] _allShows;

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
        private MvxCommand<Contact> _navigateWebLinkCommand;
        private MvxCommand<bool?> _groupByLocationCommand;
        private MvxCommand<bool?> _showHideListOptionsCommand;
        private MvxCommand<SortBy> _sortByCommand;
        private MvxCommand<string> _showFullscreenImageCommand;
        private MvxCommand _createEventCommand;
        private MvxCommand _saveEventCommand;
        private MvxCommand _cancelEventCommand;
        private MvxCommand _copyLinkCommand;
        private MvxCommand _shareCommand;

        public OrgEventViewModel(
            IClipboard clipboard,
            ISmartWalkApiService apiService,
            IConfiguration configuration,
            IAnalyticsService analyticsService,
            ICalendarService calendarService,
            IReachabilityService reachabilityService,
            IExceptionPolicy exceptionPolicy) 
            : base(reachabilityService, analyticsService)
        {
            _clipboard = clipboard;
            _apiService = apiService;
            _configuration = configuration;
            _analyticsService = analyticsService;
            _calendarService = calendarService;
            _exceptionPolicy = exceptionPolicy;
        }

        public event EventHandler<MvxValueEventArgs<string>> Share;
        public event EventHandler ZoomSelectedVenue;
        public event EventHandler ScrollSelectedVenue;

        public override string Title
        {
            get
            {
                if (OrgEvent != null && OrgEvent.StartTime.HasValue)
                {
                    var startDate = OrgEvent.StartTime.Value;
                    var endDate = OrgEvent.EndTime.HasValue 
                        ? OrgEvent.EndTime.Value 
                        : default(DateTime);

                    if (!IsMultiday || CurrentDay.HasValue)
                    {
                        var currentDate = 
                            IsMultiday
                                ? (startDate.Date.AddDays(CurrentDay.Value - 1))
                                : startDate;

                        return currentDate.GetCurrentDayString();
                    }

                    if (startDate.Month == endDate.Month)
                    {
                        return string.Format(
                            "{0}-{1} {2:MMM}", 
                            startDate.Day,
                            endDate.Day,
                            startDate);
                    }

                    return string.Format(
                        "{0:d MMMM} - {1:d MMMM}", 
                        startDate,
                        endDate);
                }

                return null;
            }
        }

        public OrgEventViewMode Mode
        {
            get
            {
                return _mode;
            }
            private set
            {
                if (_mode != value)
                {
                    _mode = value;
                    RaisePropertyChanged(() => Mode);
                }
            }
        }

        public OrgEvent OrgEvent
        {
            get
            {
                return _orgEvent;
            }
            private set
            {
                if (!Equals(_orgEvent, value))
                {
                    _orgEvent = value;
                    _allShows = null;
                    _searchableTexts = null;
                    RaisePropertyChanged(() => OrgEvent);
                    RaisePropertyChanged(() => ListItems);
                    RaisePropertyChanged(() => Title);

                    IsListOptionsAvailable = 
                        _orgEvent != null &&
                        _orgEvent.Venues != null &&
                        _orgEvent.Venues
                            .Any(v => 
                                v.Shows != null && 
                                v.Shows.Length > 0);
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

        public int? CurrentDay
        {
            get
            {
                return _currentDay;
            }
            private set
            {
                if (_currentDay != value)
                {
                    _currentDay = value;
                    RaisePropertyChanged(() => CurrentDay);
                    RaisePropertyChanged(() => CurrentDayTitle);
                    OrgEvent = GetOrgEventByDay(_allDaysOrgEvent);
                }
            }
        }

        public int DaysCount
        {
            get
            {
                return _daysCount;
            }
            private set
            {
                if (_daysCount != value)
                {
                    _daysCount = value;
                    RaisePropertyChanged(() => DaysCount);
                    RaisePropertyChanged(() => IsMultiday);
                    RaisePropertyChanged(() => CurrentDayTitle);
                }
            }
        }

        public string CurrentDayTitle
        {
            get
            {
                if (DaysCount > 1)
                {
                    if (!CurrentDay.HasValue)
                    {
                        return "All";
                    }

                    return string.Format("{0}/{1}", CurrentDay.Value, DaysCount);
                }

                return null;
            }
        }

        public bool IsMultiday
        {
            get { return DaysCount > 1; }
        }

        public Show ExpandedShow
        {
            get
            {
                return _expandedShow;
            }
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
            get
            {
                return _selectedVenueOnMap;
            }
            private set
            {
                if (!Equals(_selectedVenueOnMap, value))
                {
                    _selectedVenueOnMap = value;
                    RaisePropertyChanged(() => SelectedVenueOnMap);
                }
            }
        }

        public string CurrentFullscreenImage
        {
            get
            {
                return _currentFullscreenImage;
            }
            private set
            {
                if (_currentFullscreenImage != value)
                {
                    _currentFullscreenImage = value;
                    RaisePropertyChanged(() => CurrentFullscreenImage);
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

        public bool IsListOptionsAvailable
        {
            get
            {
                return _isListOptionsAvailable;
            }
            private set
            {
                if (_isListOptionsAvailable != value)
                {
                    _isListOptionsAvailable = value;
                    RaisePropertyChanged(() => IsListOptionsAvailable);
                }
            }
        }

        public bool IsListOptionsShown
        {
            get
            {
                return _isListOptionsShown;
            }
            private set
            {
                if (_isListOptionsShown != value)
                {
                    _isListOptionsShown = value;
                    RaisePropertyChanged(() => IsListOptionsShown);
                }
            }
        }

        public bool IsGroupedByLocation
        {
            get
            {
                return _isGroupedByLocation;
            }
            private set
            {
                if (_isGroupedByLocation != value)
                {
                    _isGroupedByLocation = value;
                    RaisePropertyChanged(() => IsGroupedByLocation);
                    RaisePropertyChanged(() => ListItems);
                }
            }
        }

        public SortBy SortBy
        {
            get
            {
                return _sortBy;
            }
            private set
            {
                if (_sortBy != value)
                {
                    _sortBy = value;
                    SortAllShowsBy();
                    RaisePropertyChanged(() => SortBy);
                    RaisePropertyChanged(() => ListItems);
                }
            }
        }

        public Venue[] SearchResults
        {
            get
            {
                return _searchResults;
            }
            private set
            {
                if (!_searchResults.EnumerableEquals(value))
                {
                    _searchResults = value;
                    RaisePropertyChanged(() => SearchResults);
                }
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
                                        venue = new Venue(match.Key.Venue.Info) { Shows = shows };
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
                        query => SearchableTexts != null);
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
                        });
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
                            OrgEvent.Id != 0);
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

                            if (ScrollSelectedVenue != null)
                            {
                                ScrollSelectedVenue(this, EventArgs.Empty);
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

                            Mode = OrgEventViewMode.Combo;
                            
                            SelectedVenueOnMap = venue;

                            if (ZoomSelectedVenue != null)
                            {
                                ZoomSelectedVenue(this, EventArgs.Empty);
                            }
                        },
                        venue => venue != null);
                }

                return _navigateVenueOnMapCommand;
            }
        }

        public ICommand NavigateWebLinkCommand
        {
            get
            {
                if (_navigateWebLinkCommand == null)
                {
                    _navigateWebLinkCommand = new MvxCommand<Contact>(
                        contact => ShowViewModel<BrowserViewModel>(
                            new BrowserViewModel.Parameters {  
                                URL = contact.ContactText,
                                Location = _parameters.Location
                            }),
                        contact => contact != null && contact.Type == ContactType.Url);
                }

                return _navigateWebLinkCommand;
            }
        }

        public ICommand ShowHideFullscreenImageCommand
        {
            get
            {
                if (_showFullscreenImageCommand == null)
                {
                    _showFullscreenImageCommand = new MvxCommand<string>(
                        image => { 
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                image != null
                                    ? Analytics.ActionLabelShowFullscreenImage
                                    : Analytics.ActionLabelHideFullscreenImage);

                            CurrentFullscreenImage = image;
                        });
                }

                return _showFullscreenImageCommand;
            }
        }

        public ICommand GroupByLocationCommand
        {
            get
            {
                if (_groupByLocationCommand == null)
                {
                    _groupByLocationCommand = new MvxCommand<bool?>(
                        groupBy => { 
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                (bool)groupBy 
                                    ? Analytics.ActionLabelTurnOnGroupByLocation
                                    : Analytics.ActionLabelTurnOffGroupByLocation);

                            IsGroupedByLocation = (bool)groupBy;
                        },
                        groupBy => groupBy.HasValue);
                }

                return _groupByLocationCommand;
            }
        }

        public ICommand ShowHideListOptionsCommand
        {
            get
            {
                if (_showHideListOptionsCommand == null)
                {
                    _showHideListOptionsCommand = new MvxCommand<bool?>(
                        on => 
                        { 
                            var isShown = on.HasValue && on.Value;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                isShown
                                    ? Analytics.ActionLabelShowListOptions
                                    : Analytics.ActionLabelHideListOptions);

                            IsListOptionsShown = isShown;
                        },
                        on => !IsLoading);
                }

                return _showHideListOptionsCommand;
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

                            IsLoading = true;

                            var eventInfo = default(OrgEvent);
                            try
                            {
                                eventInfo = await _apiService.GetOrgEventInfo(
                                    _parameters.EventId, 
                                    DataSource.Cache);
                            }
                            catch (Exception ex)
                            {
                                _exceptionPolicy.Trace(ex);
                            }
                            
                            if (eventInfo != null)
                            {
                                try
                                {
                                    CurrentCalendarEvent = 
                                        await _calendarService.CreateNewEvent(eventInfo);
                                }
                                catch (Exception ex)
                                {
                                    _exceptionPolicy.Trace(ex);
                                }
                            }

                            IsLoading = false;
                        },
                        () => 
                            _parameters != null &&
                            _parameters.EventId != 0);
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
                            _clipboard.Copy(eventUrl);
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
                    _allShows = 
                        OrgEvent.Venues
                            .SelectMany(
                                v => 
                                    v.Shows != null
                                        ? v.Shows.Select(
                                            s =>
                                            { 
                                                var venue = new Venue(v.Info) 
                                                    { 
                                                        Shows = new [] { s } 
                                                    }; 
                                                return venue;
                                            })
                                        : Enumerable.Empty<Venue>())
                            .ToArray();

                    SortAllShowsBy();
                }

                return _allShows;
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

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEvent(DataSource.Cache).ContinueWithThrow();
        }

        protected override void Refresh(DataSource source)
        {
            UpdateOrgEvent(source).ContinueWithThrow();
        }

        private async Task UpdateOrgEvent(DataSource source)
        {
            if (_parameters != null)
            {
                IsLoading = true;

                var orgEvent = default(OrgEvent);

                try 
                {
                    orgEvent = await _apiService.GetOrgEvent(
                        _parameters.EventId, 
                        source);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                IsLoading = false;

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

                RaiseRefreshCompleted(OrgEvent != null);
            }
            else
            {
                _allDaysOrgEvent = null;
                OrgEvent = null;
            }
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

                var span = endDate - startDate;
                DaysCount = span.Days + 1;

                if (startDate <= DateTime.Now.Date &&
                    DateTime.Now.Date <= endDate)
                {
                    CurrentDay = (DateTime.Now.Date - startDate).Days + 1;
                }
                else
                {
                    CurrentDay = 1; // Maybe persist day on refresh
                }
            }
        }

        private OrgEvent GetOrgEventByDay(OrgEvent orgEvent)
        {
            if (orgEvent == null || !CurrentDay.HasValue) return orgEvent;

            var firstDay = orgEvent.StartTime.Value.Date;
            var day = firstDay.AddDays(CurrentDay.Value - 1);

            var venues = 
                orgEvent.Venues
                .Select(
                    v => 
                        new Venue(v.Info)
                        { 
                            Shows = GetShowsByDay(v.Shows, day, firstDay)
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

        private Show[] GetShowsByDay(Show[] shows, DateTime day, DateTime firstDay)
        {
            if (shows == null) return null;

            var result = 
                shows
                    .Where(
                        s => s.IsShowThisDay(day, firstDay)) 
                    .ToArray();

            return result;
        }

        private Venue GetVenueByShow(Show show)
        {
            if (OrgEvent != null && show != null)
            {
                return 
                    OrgEvent.Venues
                        .FirstOrDefault(v => v.Info.Id == show.Venue
                            .First(r => r.Storage == Storage.SmartWalk).Id);
            }

            return null;
        }

        private int GetExpandedShowNumber()
        {
            if (OrgEvent != null && ExpandedShow != null)
            {
                var venue = GetVenueByShow(ExpandedShow);
                var venueNumber = Array.IndexOf(OrgEvent.Venues, venue);
                var showNumber = Array.IndexOf(venue.Shows, ExpandedShow);

                return venueNumber * 100 + showNumber;
            }

            return 0;
        }

        private void SortAllShowsBy()
        {
            if (_allShows != null)
            {
                _allShows = 
                    _allShows
                        .OrderBy(v => v.Shows[0], new ShowComparer(SortBy))
                        .ToArray();

            }
        }

        public class Parameters : ParametersBase
        {
            public int EventId { get; set; }
            public bool Current { get; set; }
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