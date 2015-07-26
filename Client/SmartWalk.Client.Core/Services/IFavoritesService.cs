using SmartWalk.Client.Core.Model;
using System;

namespace SmartWalk.Client.Core.Services
{
    public interface IFavoritesService
    {
        event EventHandler FavoritesUpdated;

        Favorites LoadFavorites();
        void SaveFavorites(Favorites favorites);
    }
}