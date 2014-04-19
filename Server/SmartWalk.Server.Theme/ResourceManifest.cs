using Orchard.UI.Resources;
using SmartWalk.Shared;

namespace SmartWalk.Server.Theme
{
    [UsedImplicitly]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("Bootstrap").SetUrl("bootstrap.min.css");
            manifest.DefineStyle("Bootstrap.Theme").SetUrl("bootstrap-theme.min.css");
            manifest.DefineStyle("SmartWalk.Theme").SetUrl("smartwalk-theme.css");

            manifest.DefineScript("Bootstrap").SetUrl("bootstrap.min.js").SetDependencies("jQuery");
        }
    }
}