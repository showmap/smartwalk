using SmartWalk.Core.ViewModels.Common;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class BrowserViewModel : ActiveViewModel
    {
        private Parameters _parameters;
        private string _browserURL;

        public BrowserViewModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
        }

        public string BrowserURL
        {
            get
            {
                return _browserURL;
            }
            private set
            {
                if (_browserURL != value)
                {
                    _browserURL = value;
                    RaisePropertyChanged(() => BrowserURL);
                }
            }
        }

        protected override object InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            BrowserURL = parameters.URL;
        }

        public class Parameters
        {
            public string URL { get; set; }
        }
    }
}