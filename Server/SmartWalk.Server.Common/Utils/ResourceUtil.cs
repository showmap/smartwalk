using System;
using Orchard.UI.Resources;

namespace SmartWalk.Server.Common.Utils
{
    public static class ResourceUtil
    {
        public static ResourceDefinition SetVersionUrl(
            this ResourceDefinition definition, 
            string url)
        {
            return definition.SetVersionUrl(url, null);
        }

        public static ResourceDefinition SetVersionUrl(
            this ResourceDefinition definition, 
            string url, 
            string urlDebug)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            if (definition.Version != null)
            {
                url += "?ver=" + definition.Version;

                if (urlDebug != null)
                {
                    urlDebug += "?ver=" + definition.Version;
                }
            }

            return definition.SetUrl(url, urlDebug);
        }
    }
}