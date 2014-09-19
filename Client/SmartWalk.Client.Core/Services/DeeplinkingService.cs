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

        public bool NavigateView(string link)
        {
            var result = false;

            if (string.IsNullOrWhiteSpace(link) ||
                link.EqualsIgnoreCase("events"))
            {
                _viewPresenter.HideDialogs();
                _viewPresenter.ShowRoot();
                result = true;
            }
            else if (link.IndexOf("event/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var id = GetId(link);
                if (id > 0)
                {
                    _viewPresenter.HideDialogs();
                    ShowViewModel<OrgEventViewModel>(
                        new OrgEventViewModel.Parameters { EventId = id });
                    result = true;
                }
            }
            else if (link.IndexOf("host/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var id = GetId(link);
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

        private static int GetId(string link)
        {
            var parts = link.Split('/');
            if (parts.Length == 2)
            {
                int id; 
                if (int.TryParse(parts[1], out id))
                {
                    return id;
                }
            }

            return 0;
        }
    }
}