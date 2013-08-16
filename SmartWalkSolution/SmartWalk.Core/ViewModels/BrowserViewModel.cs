using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.ViewModels
{
    public class BrowserViewModel : MvxViewModel
    {
        private string _browserURL;

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

        public void Init(Parameters parameters)
        {
            BrowserURL = parameters.URL;
        }

        public class Parameters
        {
            public string URL { get; set; }
        }
    }
}