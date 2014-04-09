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
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly ILocationService _locationService;

        private LocationIndex _location;
        private EntityInfo[] _orgInfos;
        private ICommand _navigateOrgViewCommand;

        public HomeViewModel(
            ISmartWalkDataService dataService,
            IExceptionPolicy exceptionPolicy,
            IAnalyticsService analyticsService,
            ILocationService locationService) : base(analyticsService)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
            _locationService = locationService;
            _locationService.LocationChanged += (s, e) => UpdateOrgInfos();
        }

        public LocationIndex Location
        {
            get
            {
                return _location;
            }
            private set
            {
                if (!Equals(_location, value))
                {
                    _location = value;
                    RaisePropertyChanged(() => Location);

                    OrgInfos = _location != null ? _location.OrgInfos : null;
                }
            }
        }

        public EntityInfo[] OrgInfos 
        {
            get
            {
                return _orgInfos;
            }
            private set
            {
                _orgInfos = value;
                RaisePropertyChanged(() => OrgInfos);
            }
        }

        public ICommand NavigateOrgViewCommand
        {
            get
            {
                if (_navigateOrgViewCommand == null)
                {
                    _navigateOrgViewCommand = new MvxCommand<EntityInfo>(
                        orgInfo => ShowViewModel<OrgViewModel>(
                            new OrgViewModel.Parameters { OrgId = orgInfo.Id }));
                }

                return _navigateOrgViewCommand;
            }
        }

        protected override object InitParameters
        {
            get { return null; }
        }

        public override void Start()
        {
            UpdateOrgInfos().ContinueWithExceptionRethrown();

            base.Start();
        }

        protected override void Refresh()
        {
            UpdateOrgInfos().ContinueWithExceptionRethrown();
        }

        private async Task UpdateOrgInfos()
        {
            if (_locationService.CurrentLocation != null)
            {
                IsLoading = true;

                var location = default(LocationIndex);

                try
                {
                    location = await _dataService.GetLocationIndex(DataSource.Server); 
                }
                catch (Exception ex)
                {
                    _exceptionPolicy.Trace(ex);
                }
                    
                IsLoading = false;

                Location = location;
                RaiseRefreshCompleted();
            }
        }
    }
}