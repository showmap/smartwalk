using System;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Services;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels.Interfaces;

namespace SmartWalk.Core.ViewModels.Common
{
    public abstract class ActiveViewModel : MvxViewModel, IActiveAware
    {
        private static readonly string ViewModelPostfix = "viewmodel";

        private readonly IAnalyticsService _analyticsService;

        private bool _isActive;

        protected ActiveViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                if (_isActive != value)
                {
                    _isActive = value;

                    if (_isActive)
                    {
                        var viewName = GetType().Name.ToLowerInvariant()
                            .Replace(ViewModelPostfix, string.Empty);

                        _analyticsService.SendView(
                            viewName,
                            InitParameters != null ? InitParameters.ToDictionary() : null);
                    }
                }
            }
        }

        protected abstract object InitParameters { get; }
    }
}