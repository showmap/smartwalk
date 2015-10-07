using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels.Interfaces;

namespace SmartWalk.Client.Core.ViewModels.Common
{
    public abstract class ModalProviderViewModel : ProgressViewModel, IModalProviderViewModel
    {
        private readonly IAnalyticsService _analyticsService;

        private MvxCommand<ModalViewContext> _showHideModalViewCommand;
        private ModalViewContext _modalViewContext;

        protected ModalProviderViewModel(IAnalyticsService analyticsService)
            : base(analyticsService)
        {
            _analyticsService = analyticsService;
        }


        public ModalViewContext ModalViewContext
        {
            get { return _modalViewContext; }
            private set
            {
                if (!Equals(_modalViewContext, value))
                {
                    _modalViewContext = value;
                    RaisePropertyChanged(() => ModalViewContext);
                }
            }
        }

        public ICommand ShowHideModalViewCommand
        {
            get
            {
                if (_showHideModalViewCommand == null)
                {
                    _showHideModalViewCommand = new MvxCommand<ModalViewContext>(context => {
                        _analyticsService.SendEvent(
                            Analytics.CategoryUI,
                            Analytics.ActionTouch,
                            context != null
                                ? Analytics.ActionLabelShowModalView
                                : Analytics.ActionLabelHideModalView,
                            context != null ? (int)context.ViewKind : 0);

                        ModalViewContext = context;
                    });
                }

                return _showHideModalViewCommand;
            }
        }
    }
}