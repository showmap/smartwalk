using System.Windows.Input;

namespace SmartWalk.Core.ViewModels.Interfaces
{
    public interface IFullscreenImageProvider
    {
        string CurrentFullscreenImage { get; }
        ICommand ShowFullscreenImageCommand { get; }
    }
}