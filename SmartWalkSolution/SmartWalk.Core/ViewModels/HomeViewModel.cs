using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;
using SmartWalk.Core.ViewModels.Common;

namespace SmartWalk.Core.ViewModels
{
    public class HomeViewModel : RefreshableViewModel
	{
		private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly ILocationService _locationService;

        private LocationIndex _location;
		private EntityInfo[] _orgInfos;
        private ICommand _navigateOrgViewCommand;
        private ICommand _navigateSettingsViewCommand;

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

        public ICommand NavigateSettingsViewCommand
        {
            get
            {
                if (_navigateSettingsViewCommand == null)
                {
                    _navigateSettingsViewCommand = new MvxCommand(
                        () => ShowViewModel<SettingsViewModel>());
                }

                return _navigateSettingsViewCommand;
            }
        }

        protected override object InitParameters
        {
            get { return null; }
        }

		public override void Start()
		{
			UpdateOrgInfos();

			base.Start();
		}

        protected override void Refresh()
        {
            UpdateOrgInfos();
        }

		private void UpdateOrgInfos()
		{
            if (_locationService.CurrentLocation != null)
            {
                IsLoading = true;

                _dataService.GetLocationIndex(DataSource.Server, (location, ex) => 
                    {
                        IsLoading = false;

                        if (ex == null)
                        {
                            Location = location;
                        }
                        else
                        {
                            _exceptionPolicy.Trace(ex);
                        }

                    
                        RaiseRefreshCompleted();
                    });
            }
		}
	}
}