using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class OrgViewModel : EntityViewModel
    {
        private readonly ISmartWalkDataService _dataService;

        private string _orgId;
        private MvxCommand _refreshCommand;

        public OrgViewModel(ISmartWalkDataService dataService)
        {
            _dataService = dataService;
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
                            // TODO: handling
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