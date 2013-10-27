using ceton.mvx.plugins.settings;
using SmartWalk.Core.Constants;
using SmartWalk.Core.Services;
using SmartWalk.Core.ViewModels.Common;

namespace SmartWalk.Core.ViewModels
{
    public class SettingsViewModel : ActiveViewModel
    {
        private readonly ISettings _settings;
        private readonly IAnalyticsService _analyticsService;

        private bool _isAnonymousStatsEnabled;

        public SettingsViewModel(ISettings settings, IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            _settings = settings;
            _analyticsService = analyticsService;

            IsAnonymousStatsEnabled = 
                _settings.GetValueOrDefault(SettingKeys.AnonymousStatsEnabled, true);
        }

        public bool IsAnonymousStatsEnabled
        {
            get { return _isAnonymousStatsEnabled; }
            set
            {
                if (_isAnonymousStatsEnabled != value)
                {
                    _isAnonymousStatsEnabled = value;
                    RaisePropertyChanged(() => IsAnonymousStatsEnabled);

                    _analyticsService.IsOptOut = !_isAnonymousStatsEnabled;

                    _settings.AddOrUpdateValue(SettingKeys.AnonymousStatsEnabled, _isAnonymousStatsEnabled);
                    _settings.Save();
                }
            }
        }

        protected override object InitParameters
        {
            get { return null; }
        }
    }
}