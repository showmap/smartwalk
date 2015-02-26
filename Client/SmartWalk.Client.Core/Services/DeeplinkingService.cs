using System;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Services
{
    public class DeeplinkingService : MvxNavigatingObject, IDeeplinkingService
    {
        private readonly IMvxExtendedViewPresenter _viewPresenter;

        public DeeplinkingService(IMvxExtendedViewPresenter viewPresenter)
        {
            _viewPresenter = viewPresenter;
        }

        public bool NavigateView(Uri url)
        {
            var result = false;

            if (url == null || url.Segments.ContainsIgnoreCase("events/"))
            {
                _viewPresenter.HideDialogs();
                _viewPresenter.ShowRoot();
                result = true;
            }
            else if (url.Segments.ContainsIgnoreCase("event/"))
            {
                var id = GetId(url, "event/");
                if (id > 0)
                {
                    _viewPresenter.HideDialogs();
                    ShowViewModel<OrgEventViewModel>(
                        new OrgEventViewModel.Parameters { EventId = id });
                    result = true;
                }
            }
            else if (url.Segments.ContainsIgnoreCase("organizer/"))
            {
                var id = GetId(url, "organizer/");
                if (id > 0)
                {
                    _viewPresenter.HideDialogs();
                    ShowViewModel<OrgViewModel>(
                        new OrgViewModel.Parameters { OrgId = id });
                    result = true;
                }
            }

            return result;
        }

        private static int GetId(Uri url, string param)
        {
            var paramIndex = Array.IndexOf(url.Segments, param);
            if (url.Segments.Length > paramIndex + 1)
            {
                int id; 
                if (int.TryParse(url.Segments[paramIndex + 1], out id))
                {
                    return id;
                }
            }

            return 0;
        }
    }
}