using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using Cirrious.CrossCore.Core;

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

        private OrgEventViewMode _mode = OrgEventViewMode.List;
        private OrgEvent _orgEvent;
        private Show _expandedShow;
        private Venue _selectedVenueOnMap;
        private string _currentFullscreenImage;
        private CalendarEvent _currentCalendarEvent;
        private Parameters _parameters;
        private bool _isGroupedByLocation = true;

        private MvxCommand<Show> _expandCollapseShowCommand;
        private MvxCommand<OrgEventViewMode?> _switchModeCommand;
        private MvxCommand _navigateOrgCommand;
        private MvxCommand _navigateOrgEventInfoCommand;
        private MvxCommand<Venue> _navigateVenueCommand;
        private MvxCommand<Venue> _navigateVenueOnMapCommand;
        private MvxCommand<Contact> _navigateWebLinkCommand;
        private MvxCommand<bool?> _groupByLocationCommand;
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
            IExceptionPolicy exceptionPolicy) : base(analyticsService)
        {
            _clipboard = clipboard;
            _apiService = apiService;
            _configuration = configuration;
            _analyticsService = analyticsService;
            _calendarService = calendarService;
            _exceptionPolicy = exceptionPolicy;
        }

        public event EventHandler<MvxValueEventArgs<string>> Share;

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
                    RaisePropertyChanged(() => OrgEvent);
                }
            }
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
                }
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
                                    case OrgEventViewMode.List:
                                        Mode = OrgEventViewMode.Map;
                                        SelectedVenueOnMap = null;
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
                                    : Analytics.ActionLabelSwitchToMap);
                        });
                }

                return _switchModeCommand;
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
                            _parameters.IsCurrentEvent &&
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
                                OrgEventId = OrgEvent.Id,
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
                                OrgEventId = _parameters.OrgEventId,
                                Location = _parameters.Location
                            }),
                        venue => 
                            venue != null && 
                            _parameters != null &&
                            _parameters.OrgEventId != 0);
                }

                return _navigateVenueCommand;
            }
        }

        public ICommand NavigateVenueOnMapCommand
        {
            get
            {
                if (_navigateVenueOnMapCommand == null)
                {
                    _navigateVenueOnMapCommand = new MvxCommand<Venue>(
                        venue => {
                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelNavigateVenueOnMap,
                                venue.Info.Id);

                            Mode = OrgEventViewMode.Map;
                            SelectedVenueOnMap = venue;
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
                                    _parameters.OrgEventId, 
                                    DataSource.Cache);
                            }
                            catch (Exception ex)
                            {
                                _exceptionPolicy.Trace(ex);
                            }
                            
                            try
                            {
                                CurrentCalendarEvent = 
                                    await _calendarService.CreateNewEvent(eventInfo);
                            }
                            catch (Exception ex)
                            {
                                _exceptionPolicy.Trace(ex);
                            }

                            IsLoading = false;
                        },
                        () => 
                            _parameters != null &&
                            _parameters.OrgEventId != 0);
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

                            var eventUrl = _configuration.GetEventUrl(_parameters.OrgEventId);
                            _clipboard.Copy(eventUrl);
                        },
                        () => 
                            _parameters != null &&
                            _parameters.OrgEventId != 0);
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

                            var eventUrl = _configuration.GetEventUrl(_parameters.OrgEventId);
                            if (eventUrl != null && Share != null)
                            {
                                Share(this, new MvxValueEventArgs<string>(eventUrl));
                            }
                        },
                        () => 
                            _parameters != null &&
                            _parameters.OrgEventId != 0);
                }

                return _shareCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEvent(DataSource.Cache).ContinueWithThrow();
        }

        protected override void Refresh()
        {
            UpdateOrgEvent(DataSource.Server).ContinueWithThrow();
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
                        _parameters.OrgEventId, 
                        source);
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }

                IsLoading = false;

                OrgEvent = orgEvent;
                RaiseRefreshCompleted();
            }
            else
            {
                OrgEvent = null;
            }
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

        public class Parameters : ParametersBase
        {
            public int OrgEventId { get; set; }
            public bool IsCurrentEvent { get; set; }
        }
    }

    public enum OrgEventViewMode
    {
        List,
        Map
    }
}