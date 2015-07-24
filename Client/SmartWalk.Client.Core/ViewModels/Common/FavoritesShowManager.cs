using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.Core.ViewModels.Common
{
    public class FavoritesShowManager
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly List<int> _favoriteShows = new List<int>();

        private MvxCommand<Show> _setFavoriteShowCommand;
        private MvxCommand<Show> _unsetFavoriteShowCommand;

        public FavoritesShowManager(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public ICommand SetFavoriteShowCommand
        {
            get
            {
                if (_setFavoriteShowCommand == null)
                {
                    _setFavoriteShowCommand = new MvxCommand<Show>(show => 
                        {
                            var showIds = show.GetShowIds();

                            foreach(var showId in showIds)
                            {
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelSetFavoriteShow,
                                    showId);

                                if (!_favoriteShows.Contains(showId))
                                {
                                    _favoriteShows.Add(showId);
                                }
                            }
                        },
                        show => show != null);
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
                    _unsetFavoriteShowCommand = new MvxCommand<Show>(show => 
                        {
                            var showIds = show.GetShowIds();

                            foreach(var showId in showIds)
                            {
                                _analyticsService.SendEvent(
                                    Analytics.CategoryUI,
                                    Analytics.ActionTouch,
                                    Analytics.ActionLabelUnsetFavoriteShow,
                                    showId);

                                if (_favoriteShows.Contains(showId))
                                {
                                    _favoriteShows.Remove(showId);
                                }
                            }
                        },
                        show => show != null);
                }

                return _unsetFavoriteShowCommand;
            }
        }

        public bool IsShowFavorite(Show show)
        {
            var showIds = show.GetShowIds();
            var result = _favoriteShows.Any(fs => showIds.Contains(fs));
            return result;
        }
    }
}