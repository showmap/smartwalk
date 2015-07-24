using SmartWalk.Client.Core.ViewModels.Common;

namespace SmartWalk.Client.Core.ViewModels.Interfaces
{
    public interface IFavoritesAware
    {
        FavoritesShowManager FavoritesManager { get; }
    }
}