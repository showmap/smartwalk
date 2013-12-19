using SmartWalk.Client.Core.ViewModels.Interfaces;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.Core.ViewModels.Common
{
    public abstract class ProgressViewModel : ActiveViewModel, IProgressViewModel
    {
        private bool _isLoading;

        protected ProgressViewModel(IAnalyticsService analyticsService) : 
            base(analyticsService)
        {
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            protected set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged(() => IsLoading);
                }
            }
        }
    }
}

