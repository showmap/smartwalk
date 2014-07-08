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

            // jQuery

            manifest.DefineScript("jQuery")
                .SetVersion("2.1.1")
                .SetVersionUrl("jquery-2.1.1.min.js", "jquery-2.1.1.js")
                .SetCdn(
                    "//code.jquery.com/jquery-2.1.1.min.js",
                    "//code.jquery.com/jquery-2.1.1.js",
                    true);

            // Bootstrap

            manifest.DefineStyle("Bootstrap")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap.min.css", "bootstrap.css")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css");

            manifest.DefineStyle("Bootstrap.Theme")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap-theme.min.css", "bootstrap-theme.css")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css")
                .SetDependencies("Bootstrap");

            manifest.DefineScript("Bootstrap")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap.min.js", "bootstrap.js")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js")
                .SetDependencies("jQuery");

            // SmartWalk

            // TODO: To setup *.min.css
            manifest.DefineStyle("SmartWalk.Theme")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-theme.css")
                .SetDependencies("Bootstrap.Theme");
        }
    }
}