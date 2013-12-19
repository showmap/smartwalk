using System.Windows.Input;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IContactsEntityProvider
    {
        EntityInfo CurrentContactsEntityInfo { get; }
        ICommand ShowHideContactsCommand { get; }
        ICommand NavigateWebLinkCommand { get; }
        ICommand CallPhoneCommand { get; }
        ICommand ComposeEmailCommand { get; }
    }
}