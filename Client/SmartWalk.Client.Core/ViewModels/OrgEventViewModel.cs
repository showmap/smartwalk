using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.Constants;

namespace SmartWalk.Client.Core.ViewModels
{
    public class OrgEventViewModel : RefreshableViewModel, IFullscreenImageProvider
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private OrgEventViewMode _mode = OrgEventViewMode.List;
        private OrgEvent _orgEvent;
        private VenueShow _expandedShow;
        private Venue _selectedVenueOnMap;
        private string _currentFullscreenImage;
        private Parameters _parameters;
        private bool _isGroupedByLocation = true;

        private MvxCommand<VenueShow> _expandCollapseShowCommand;
        private MvxCommand<OrgEventViewMode?> _switchModeCommand;
        private MvxCommand<Venue> _navigateVenueCommand;
        private MvxCommand<Venue> _navigateVenueOnMapCommand;
        private MvxCommand<WebSiteInfo> _navigateWebLinkCommand;
        private MvxCommand<bool?> _groupByLocationCommand;
        private MvxCommand<string> _showFullscreenImageCommand;

        public OrgEventViewModel(
            ISmartWalkDataService dataService,
            IAnalyticsService analyticsService,
            IExceptionPolicy exceptionPolicy) : base(analyticsService)
        {
            _dataService = dataService;
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

        public VenueShow ExpandedShow
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
                    _expandCollapseShowCommand = new MvxCommand<VenueShow>(show => 
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
                                OrgId = _parameters.OrgId, 
                                EventDate = _parameters.Date,
                                VenueNumber = venue.Number,
                                VenueName = venue.Info.Name
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
                                venue.Number);
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
                    _navigateWebLinkCommand = new MvxCommand<WebSiteInfo>(
                        info => ShowViewModel<BrowserViewModel>(
                            new BrowserViewModel.Parameters {  
                                URL = info.URL
                        }),
                        info => info != null);
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

            UpdateOrgEvent();
        }

        protected override void Refresh()
        {
            UpdateOrgEvent();
        }

        private void UpdateOrgEvent()
        {
            if (_parameters != null)
            {
                IsLoading = true;

                _dataService.GetOrgEvent(
                    _parameters.OrgId, 
                    _parameters.Date, 
                    DataSource.Server, 
                    (orgEvent, ex) => 
                {
                    IsLoading = false;

                    if (ex == null)
                    {
                        OrgEvent = orgEvent;
                    }
                    else
                    {
                        _exceptionPolicy.Trace(ex);
                    }
                    
                    RaiseRefreshCompleted();
                });
            }
            else
            {
                OrgEvent = null;
            }
        }

        private Venue GetVenueByShow(VenueShow show)
        {
            if (OrgEvent != null && show != null)
            {
                return OrgEvent.Venues.FirstOrDefault(v => v.Shows.Contains(show));
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
            public string OrgId { get; set; }

            public DateTime Date { get; set; }
        }
    }

    public enum OrgEventViewMode
    {
        List,
        Map
    }
}