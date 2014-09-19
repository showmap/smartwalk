using Cirrious.MvvmCross.Views;

namespace SmartWalk.Client.Core.Services
{
    public interface IMvxExtendedViewPresenter : IMvxViewPresenter
    {
        void HideDialogs();
        void ShowRoot();
    }
}