using System;
using System.Windows.Input;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels.Common;
using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.ViewModels
{
    public class BrowserViewModel : ActiveViewModel, IShareableViewModel
    {
        private const string Http = "http://";
        private const string Https = "https://";

        private readonly IEnvironmentService _environmentService;
        private readonly IAnalyticsService _analyticsService;

        private Parameters _parameters;
        private string _browserURL;
        private MvxCommand _openLinkCommand;
        private MvxCommand _copyLinkCommand;
        private MvxCommand _shareCommand;

        public BrowserViewModel(
            IEnvironmentService environmentService,
            IAnalyticsService analyticsService) : base(analyticsService)
        {
            _environmentService = environmentService;
            _analyticsService = analyticsService;
        }

        public event EventHandler<MvxValueEventArgs<string>> Share;

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

        public ICommand OpenLinkCommand
        {
            get
            {
                if (_openLinkCommand == null)
                {
                    _openLinkCommand = new MvxCommand(() => 
                    {
                        _analyticsService.SendEvent(
                            Analytics.CategoryUI,
                            Analytics.ActionTouch,
                            Analytics.ActionLabelOpenLink);

                        _environmentService.OpenURL(BrowserURL);
                    },
                    () => BrowserURL != null);
                }

                return _openLinkCommand;
            }
        }

        public ICommand CopyLinkCommand
        {
            get
            {
                if (_copyLinkCommand == null)
                {
                    _copyLinkCommand = new MvxCommand(() => 
                    {
                        _analyticsService.SendEvent(
                            Analytics.CategoryUI,
                            Analytics.ActionTouch,
                            Analytics.ActionLabelCopyLink);

                        _environmentService.Copy(BrowserURL);
                    },
                    () => BrowserURL != null);
                }

                return _copyLinkCommand;
            }
        }

        public ICommand ShareCommand
        {
            get
            {
                if (_shareCommand == null)
                {
                    _shareCommand = new MvxCommand(() => 
                    {
                        _analyticsService.SendEvent(
                            Analytics.CategoryUI,
                            Analytics.ActionTouch,
                            Analytics.ActionLabelShare);

                        if (Share != null)
                        {
                            Share(this, new MvxValueEventArgs<string>(BrowserURL));
                        }
                    },
                    () => BrowserURL != null);
                }

                return _shareCommand;
            }
        }

        protected override ParametersBase InitParameters
        {
            get { return _parameters; }
        }

        public void Init(Parameters parameters)
        {
            _parameters = parameters;

            BrowserURL = GetFixedURL(parameters.URL);
        }

        private static string GetFixedURL(string url)
        {
            if (url != null)
            {
                if (!url.StartsWith(Http, StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith(Https, StringComparison.OrdinalIgnoreCase))
                {
                    url = Http + url;
                }

                return url;
            }

            return null;
        }

        public class Parameters : ParametersBase
        {
            public string URL { get; set; }

            public override bool Equals(object obj)
            {
                var parameters = obj as Parameters;
                if (parameters != null)
                {
                    return Equals(Location, parameters.Location) && 
                        Equals(URL, parameters.URL);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Initial
                    .CombineHashCodeOrDefault(Location)
                    .CombineHashCodeOrDefault(URL);
            }
        }
    }
}