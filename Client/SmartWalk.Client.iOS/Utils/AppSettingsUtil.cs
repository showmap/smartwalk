using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GoogleAnalytics;
using MonoTouch.Foundation;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Services;

namespace SmartWalk.Client.iOS.Utils
{
    public static class AppSettingsUtil
    {
        public static AppSettings LoadSettings()
        {
            using (var reader = XmlReader.Create("settings.xml"))
            {
                var serializer = new XmlSerializer(typeof(AppSettings));
                var settings = (AppSettings)serializer.Deserialize(reader);
                return settings;
            }
        }

        public static void HandleResetCache(AppSettings settings)
        {
            if (NSUserDefaults.StandardUserDefaults[SettingKeys.ResetCache] != null &&
                NSUserDefaults.StandardUserDefaults.BoolForKey(SettingKeys.ResetCache))
            {
                try
                {
                    if (Directory.Exists(settings.CachesPath))
                    {
                        Directory.Delete(settings.CachesPath, true);
                    }

                    NSUserDefaults.StandardUserDefaults.SetBool(false, SettingKeys.ResetCache);
                }
                catch(Exception ex)
                {
                    if (!GoogleAnalyticsService.IsOptOut)
                    {
                        EasyTracker.Current
                            .OnApplicationUnhandledException(ex)
                            .ContinueWithThrow();
                    }
                }
            }
        }
    }
}