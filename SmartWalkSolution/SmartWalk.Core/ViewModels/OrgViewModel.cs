using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class OrgViewModel : EntityViewModel
    {
        private readonly ISmartWalkDataService _dataService;
        private readonly IExceptionPolicy _exceptionPolicy;

        private string _orgId;
        private ICommand _refreshCommand;
        private ICommand _navigateOrgEventViewCommand;

        public OrgViewModel(ISmartWalkDataService dataService, IExceptionPolicy exceptionPolicy)
        {
            _dataService = dataService;
            _exceptionPolicy = exceptionPolicy;
        }

        public Org Org
        {
            get
            {
                return (Org)Entity;
            }
            private set
            {
                if (!Equals(Entity, value))
                {
                    Entity = value;
                    RaisePropertyChanged(() => Org);
                }
            }
        }

        public ICommand NavigateOrgEventViewCommand
        {
            get
            {
                if (_navigateOrgEventViewCommand == null)
                {
                    _navigateOrgEventViewCommand = new MvxCommand<OrgEventInfo>(
                        evenInfo => ShowViewModel<OrgEventViewModel>(
                        new OrgEventViewModel.Parameters {  
                            OrgId = evenInfo.OrgId, 
                            Date = evenInfo.Date
                        }));
                }

                return _navigateOrgEventViewCommand;
            }
        }

        public ICommand RefreshCommand
        {
            get 
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new MvxCommand(UpdateOrg);
                }

                return _refreshCommand;
            }
        }

        public void Init(Parameters parameters)
        {
            _orgId = parameters.OrgId;

            UpdateOrg();
        }

        private void UpdateOrg()
        {
            if (_orgId != null)
            {
                _dataService.GetOrg(_orgId, (org, ex) => 
                    {
                        if (ex == null)
                        {
                            Org = org;
                        }
                        else
                        {
                            _exceptionPolicy.Trace(ex);
                        }
                    });
            }
            else
            {
                Org = null;
            }
        }

        public class Parameters
        {
            public string OrgId { get; set; }
        }
    }
}