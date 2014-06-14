using System;
using Cirrious.MvvmCross.Plugins.DownloadCache.Touch;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Utils.MvvmCross
{
    public class CacheConfiguration : MvxDownloadCacheConfiguration, ICacheConfiguration
    {
        private int _maxFileAgeDays;
        private int _maxFileAgeHours;

        public int MaxFileAgeDays
        {
            get
            {
                return _maxFileAgeDays;
            }
            set
            {
                _maxFileAgeDays = value;
                MaxFileAge = TimeSpan.FromDays(_maxFileAgeDays);
            }
        }

        public int MaxFileAgeHours
        {
            get
            {
                return _maxFileAgeHours;
            }
            set
            {
                _maxFileAgeHours = value;
                MaxFileAge = TimeSpan.FromHours(_maxFileAgeHours);
            }
        }
    }
}