using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
	{
		private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

		private EntityInfo[] _orgInfos;
        private ICommand _refreshCommand;
        private ICommand _navigateOrgViewCommand;

        public HomeViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
		{
			_dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
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

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(UpdateOrgInfos);
                }

                return _refreshCommand;
            }
        }

		public override void Start()
		{
			UpdateOrgInfos();

			base.Start();
		}

		private void UpdateOrgInfos()
		{
			_dataService.GetOrgInfos((orgInfos, ex) => 
          		{
					if (ex == null) 
					{
						OrgInfos = orgInfos;
					}
					else 
					{
                        _exceptionPolicy.Trace(ex);
					}
				});
		}
	}
}