using Orchard.UI.Resources;
using SmartWalk.Server.Common.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server.Theme
{
    [UsedImplicitly]
    public class ResourceManifest : IResourceManifestProvider
    {
        public static string GetCurrentVersion()
        {
            return VersionUtil.CurrentVersion;
        }

        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("Bootstrap")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap.min.css", "bootstrap.css")
                .SetCdn("http://netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css");

            manifest.DefineStyle("Bootstrap.Theme")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap-theme.min.css", "bootstrap-theme.css")
                .SetCdn("http://netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css");

            // TODO: To setup *.min.css
            manifest.DefineStyle("SmartWalk.Theme")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-theme.css");

            manifest.DefineScript("Bootstrap")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap.min.js", "bootstrap.js")
                .SetCdn("http://netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js")
                .SetDependencies("jQuery");
        }
    }
}