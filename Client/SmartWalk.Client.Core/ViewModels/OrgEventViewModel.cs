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

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgEventViewModel : RefreshableViewModel, IFullscreenImageProvider
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private OrgEventViewMode _mode = OrgEventViewMode.List;
        private OrgEvent _orgEvent;
        private Show _expandedShow;
        private Venue _selectedVenueOnMap;
        private string _currentFullscreenImage;
        private Parameters _parameters;
        private bool _isGroupedByLocation = true;

        private MvxCommand<Show> _expandCollapseShowCommand;
        private MvxCommand<OrgEventViewMode?> _switchModeCommand;
        private MvxCommand<Venue> _navigateVenueCommand;
        private MvxCommand<Venue> _navigateVenueOnMapCommand;
        private MvxCommand<Contact> _navigateWebLinkCommand;
        private MvxCommand<bool?> _groupByLocationCommand;
        private MvxCommand<string> _showFullscreenImageCommand;

        public OrgEventViewModel(
            ISmartWalkApiService apiService,
            IAnalyticsService analyticsService,
            IExceptionPolicy exceptionPolicy) : base(analyticsService)
        {
            _apiService = apiService;
            _analyticsService = analyticsService;
            _exceptionPolicy = exceptionPolicy;
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
                            if (!Equals(ExpandedShow, show))
                            {
                                ExpandedShow = show;
                            }
                            else 
                            {
                                ExpandedShow = null;
                            }

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
                                OrgEventId = _parameters.OrgEventId
                            }),
                        venue => venue != null && _parameters != null);
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
                            Mode = OrgEventViewMode.Map;
                            SelectedVenueOnMap = venue;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                Analytics.ActionLabelNavigateVenueOnMap,
                                venue.Info.Id);
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
                                URL = contact.ContactText
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
                            CurrentFullscreenImage = image;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                CurrentFullscreenImage != null
                                    ? Analytics.ActionLabelShowFullscreenImage
                                    : Analytics.ActionLabelHideFullscreenImage);
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
                            IsGroupedByLocation = (bool)groupBy;

                            _analyticsService.SendEvent(
                                Analytics.CategoryUI,
                                Analytics.ActionTouch,
                                IsGroupedByLocation 
                                    ? Analytics.ActionLabelTurnOnGroupByLocation
                                    : Analytics.ActionLabelTurnOffGroupByLocation);
                        },
                        groupBy => groupBy.HasValue);
                }

                return _groupByLocationCommand;
            }
        }

        protected override object InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEvent().ContinueWithThrow();
        }

        protected override void Refresh()
        {
            UpdateOrgEvent().ContinueWithThrow();
        }

        private async Task UpdateOrgEvent()
        {
            if (_parameters != null)
            {
                IsLoading = true;

                var orgEvent = default(OrgEvent);

                try 
                {
                    orgEvent = await _apiService.GetOrgEvent(
                        _parameters.OrgEventId, 
                        DataSource.Server);
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

        public class Parameters
        {
            public int OrgEventId { get; set; }
        }
    }

    public enum OrgEventViewMode
    {
        List,
        Map
    }
}