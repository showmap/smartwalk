using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GoogleAnalytics;
using MonoTouch.Foundation;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Services;

namespace SmartWalk.Client.iOS.Utils
{
    public static class SettingsUtil
    {
        public static Settings LoadSettings()
        {
            using (var reader = XmlReader.Create("settings.xml"))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                var settings = (Settings)serializer.Deserialize(reader);
                return settings;
            }
        }

        public static void HandleResetCache(Settings settings)
        {
            if (NSUserDefaults.StandardUserDefaults[SettingKeys.ResetCache] != null &&
                NSUserDefaults.StandardUserDefaults.BoolForKey(SettingKeys.ResetCache))
            {
                try
                {
                    foreach (var cache in settings.Caches)
                    {
                        if (Directory.Exists(cache.CacheFolderPath))
                        {
                            Directory.Delete(cache.CacheFolderPath, true);
                        }
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