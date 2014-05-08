using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;

namespace SmartWalk.Client.Core.ViewModels
{
    public class HomeViewModel : RefreshableViewModel
    {
        private readonly ISmartWalkApiService _apiService;
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly ILocationService _locationService;
        private readonly Parameters _parameters;

        private string _locationString;
        private OrgEvent[] _eventInfos;
        private ICommand _navigateOrgEventViewCommand;

        public HomeViewModel(
            ISmartWalkApiService apiService,
            IExceptionPolicy exceptionPolicy,
            IAnalyticsService analyticsService,
            IReachabilityService reachabilityService,
            ILocationService locationService) 
            : base(reachabilityService, analyticsService)
        {
            _apiService = apiService;
            _exceptionPolicy = exceptionPolicy;
            _locationService = locationService;

            _parameters = new Parameters();

            _locationService.LocationChanged += (s, e) => Refresh(DataSource.Server);
            _locationService.LocationStringChanged += (s, e) => UpdateLocationString();

            UpdateLocationString();
        }

        public override string Title
        {
            get { return LocationString; }
        }

        public string LocationString
        {
            get
            {
                return _locationString;
            }
            private set
            {
                if (!Equals(_locationString, value))
                {
                    _locationString = value;
                    RaisePropertyChanged(() => LocationString);
                    RaisePropertyChanged(() => Title);

                    _parameters.Location = _locationString;
                }
            }
        }

        public OrgEvent[] EventInfos 
        {
            get
            {
                return _eventInfos;
            }
            private set
            {
                _eventInfos = value;
                RaisePropertyChanged(() => EventInfos);
            }
        }

        public ICommand NavigateOrgEventViewCommand
        {
            get
            {
                if (_navigateOrgEventViewCommand == null)
                {
                    _navigateOrgEventViewCommand = new MvxCommand<OrgEvent>(
                        orgEvent => ShowViewModel<OrgEventViewModel>(
                            new OrgEventViewModel.Parameters { 
                                EventId = orgEvent.Id,
                                Current = true,
                                Location = _parameters.Location
                            }));
                }

                return _navigateOrgEventViewCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public override void Start()
        {
            UpdateEventInfos(DataSource.Cache).ContinueWithThrow();

            base.Start();
        }

        protected override void Refresh(DataSource source = DataSource.Server)
        {
            UpdateEventInfos(source).ContinueWithThrow();
        }

        private async Task UpdateEventInfos(DataSource source)
        {
            IsLoading = true;

            var eventInfos = default(OrgEvent[]);

            try
            {
                eventInfos = await _apiService.GetOrgEvents(
                    _locationService.CurrentLocation,
                    source); 
            }
            catch (Exception ex)
            {
                _exceptionPolicy.Trace(ex);
            }
                
            IsLoading = false;

            EventInfos = eventInfos;
            RaiseRefreshCompleted();
        }

        private void UpdateLocationString()
        {
            LocationString = _locationService.CurrentLocationString;
        }

        public class Parameters : ParametersBase
        {
        }
    }
}