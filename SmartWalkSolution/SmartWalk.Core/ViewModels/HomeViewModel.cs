using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;
using System.Windows.Input;

namespace SmartWalk.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
	{
		private readonly ISmartWalkDataService _dataService;

		private IEnumerable<EntityInfo> _orgInfos;

		public HomeViewModel(ISmartWalkDataService dataService)
		{
			_dataService = dataService;
		}

		public IEnumerable<EntityInfo> OrgInfos 
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
                return new MvxCommand<EntityInfo>(orgInfo => ShowViewModel<OrgViewModel>(
                    new OrgViewModel.Parameters { OrgId = orgInfo.Id }));
            }
        }

        public ICommand RefreshCommand
        {
            get 
            {
                return new MvxCommand(() => UpdateOrgInfos());
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
						// TODO: handling
					}
				});
		}
	}
}