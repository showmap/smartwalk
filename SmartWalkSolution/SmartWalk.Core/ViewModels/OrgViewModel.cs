using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class OrgViewModel : MvxViewModel
    {
        private readonly ISmartWalkDataService _dataService;

        private Org _org;
        private string _orgId;

        public OrgViewModel(ISmartWalkDataService dataService)
        {
            _dataService = dataService;
        }

        public Org Org
        {
            get
            {
                return _org;
            }
            private set
            {
                if (!Equals(_org, value))
                {
                    _org = value;
                    RaisePropertyChanged(() => Org);
                }
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