using System;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;
using MonoTouch.Foundation;

namespace SmartWalk.Core.ViewModels
{
    [Preserve(AllMembers=true)]
    public class OrgEventViewModel : MvxViewModel, IRefreshableViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private OrgEventViewMode _mode = OrgEventViewMode.List;
        private OrgEvent _orgEvent;
        private Venue _selectedVenueOnMap;
        private MvxCommand _refreshCommand;
        private MvxCommand<OrgEventViewMode?> _switchModeCommand;
        private MvxCommand<Venue> _navigateVenueCommand;
        private MvxCommand<Venue> _navigateVenueOnMapCommand;
        private Parameters _parameters;

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