using System.Windows.Input;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IFullscreenImageProvider
    {
        string CurrentFullscreenImage { get; }
        ICommand ShowHideFullscreenImageCommand { get; }
    }
}