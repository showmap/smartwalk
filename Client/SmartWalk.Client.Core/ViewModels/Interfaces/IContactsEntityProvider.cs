using System.Windows.Input;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IContactsEntityProvider
    {
        Entity CurrentContactsEntityInfo { get; }
        ICommand ShowHideContactsCommand { get; }
        ICommand NavigateWebLinkCommand { get; }
        ICommand CallPhoneCommand { get; }
        ICommand ComposeEmailCommand { get; }
    }
}