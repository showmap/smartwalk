using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.ViewModels
{
    public class ProgressViewModel : MvxViewModel
    {
        private bool _isLoading;

        protected ProgressViewModel()
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

