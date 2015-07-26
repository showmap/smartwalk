using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.ViewModels.Common
{
    public class FavoritesShowManager
    {
        private readonly IFavoritesService _favoritesService;
        private readonly IAnalyticsService _analyticsService;

        private Favorites _favorites;
        private MvxCommand<Tuple<int, Show>> _setFavoriteShowCommand;
        private MvxCommand<Tuple<int, Show>> _unsetFavoriteShowCommand;

        public FavoritesShowManager(IFavoritesService favoritesService,
            IAnalyticsService analyticsService)
        {
            _favoritesService = favoritesService;
            _analyticsService = analyticsService;

            _favorites = _favoritesService.LoadFavorites();

            _favoritesService.FavoritesUpdated += (s, e) => {
                _favorites = _favoritesService.LoadFavorites();  

                if (FavoritesUpdated != null)
                {
                    FavoritesUpdated(this, EventArgs.Empty);
                }
            };
        }

        public event EventHandler FavoritesUpdated;

        public ICommand SetFavoriteShowCommand
        {
            get
            {
                if (_setFavoriteShowCommand == null)
                {
                    _setFavoriteShowCommand = new MvxCommand<Tuple<int, Show>>(tuple => 
                        {
                            var showIds = tuple.Item2.GetShowIds();
                            var updated = false;

                            foreach(var showId in showIds)
                            {
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelSetFavoriteShow,
                                    showId);

                                if (_favorites.AddEventShow(tuple.Item1, showId))
                                {
                                    updated = true;
                                }
                            }

                            if (updated)
                            {
                                _favoritesService.SaveFavorites(_favorites);

                                if (FavoritesUpdated != null)
                                {
                                    FavoritesUpdated(this, EventArgs.Empty);
                                }
                            }
                        },
                        tuple => tuple != null);
                }

                return _setFavoriteShowCommand;
            }
        }

        public ICommand UnsetFavoriteShowCommand
        {
            get
            {
                if (_unsetFavoriteShowCommand == null)
                {
                    _unsetFavoriteShowCommand = new MvxCommand<Tuple<int, Show>>(tuple => 
                        {
                            var showIds = tuple.Item2.GetShowIds();
                            var updated = false;

                            foreach(var showId in showIds)
                            {
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelUnsetFavoriteShow,
                                    showId);

                                if (_favorites.RemoveEventShow(tuple.Item1, showId))
                                {
                                    updated = true;
                                }
                            }

                            if (updated)
                            {
                                _favoritesService.SaveFavorites(_favorites);

                                if (FavoritesUpdated != null)
                                {
                                    FavoritesUpdated(this, EventArgs.Empty);
                                }
                            }
                        },
                        tuple => tuple != null);
                }

                return _unsetFavoriteShowCommand;
            }
        }

        public bool IsShowFavorite(int orgEventId, Show show)
        {
            var showIds = show.GetShowIds();
            var result = _favorites.GetEvent(orgEventId).ShowIds
                .Any(fs => showIds.Contains(fs));
            return result;
        }
    }
}