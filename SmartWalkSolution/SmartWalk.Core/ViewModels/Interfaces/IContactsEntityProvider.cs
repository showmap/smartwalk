using System.Windows.Input;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.ViewModels.Interfaces
{
    public interface IContactsEntityProvider
    {
        EntityInfo CurrentContactsEntityInfo { get; }
        ICommand ShowHideContactsCommand { get; }
        ICommand NavigateWebLinkCommand { get; }
    }
}