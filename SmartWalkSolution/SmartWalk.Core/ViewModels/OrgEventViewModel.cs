using System;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class OrgEventViewModel : MvxViewModel, IRefreshableViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private OrgEventViewMode _mode = OrgEventViewMode.List;
        private OrgEvent _orgEvent;
        private VenueShow _expandedShow;
        private Venue _selectedVenueOnMap;
        private MvxCommand<VenueShow> _expandCollapseShowCommand;
        private MvxCommand _refreshCommand;
        private MvxCommand<OrgEventViewMode?> _switchModeCommand;
        private MvxCommand<Venue> _navigateVenueCommand;
        private MvxCommand<Venue> _navigateVenueOnMapCommand;
        private MvxCommand<bool?> _groupByLocationCommand;
        private Parameters _parameters;
        private bool _isGroupedByLocation = true;

        public OrgEventViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
        }

        public event EventHandler RefreshCompleted;

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

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(UpdateOrgEvent);
                }

                return _refreshCommand;
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
                        },
                        venue => _parameters != null);
                }

                return _navigateVenueOnMapCommand;
            }
        }

        public ICommand GroupByLocationCommand
        {
            get
            {
                if (_groupByLocationCommand == null)
                {
                    _groupByLocationCommand = new MvxCommand<bool?>(
                        groupBy => IsGroupedByLocation = (bool)groupBy,
                        groupBy => groupBy.HasValue);
                }

                return _groupByLocationCommand;
            }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            UpdateOrgEvent();
        }

        private void UpdateOrgEvent()
        {
            if (_parameters != null)
            {
                _dataService.GetOrgEvent(
                    _parameters.OrgId, 
                    _parameters.Date, 
                    DataSource.Server, 
                    (orgEvent, ex) => 
                {
                    if (ex == null)
                    {
                        OrgEvent = orgEvent;

                        if (RefreshCompleted != null)
                        {
                            RefreshCompleted(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        _exceptionPolicy.Trace(ex);
                    }
                });
            }
            else
            {
                OrgEvent = null;
            }
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