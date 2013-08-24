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

        private Location _location;
		private EntityInfo[] _orgInfos;
        private ICommand _navigateOrgViewCommand;

        public HomeViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
		{
			_dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
		}

        public Location Location
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
            IsLoading = true;

            // TODO: Use location for getting orgs
            _dataService.GetLocation("sfba", DataSource.Server, (location, ex) => 
          		{
                    IsLoading = false;

					if (ex == null) 
					{
                        Location = location;
                        RaiseRefreshCompleted();
					}
					else 
					{
                        _exceptionPolicy.Trace(ex);
					}
				});
		}
	}
}