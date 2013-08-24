using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.ViewModels.Interfaces;

namespace SmartWalk.Core.ViewModels.Common
{
    public class ProgressViewModel : MvxViewModel, IProgressViewModel
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

