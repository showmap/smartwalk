using SmartWalk.Shared;
using Orchard.UI.Resources;

namespace SmartWalk.Server
{
    [UsedImplicitly]
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // TODO: To setup *.min.css and version
            manifest.DefineStyle("SmartWalk").SetUrl("smartwalk.css");

            manifest.DefineScript("SmartWalk.AntiForgery").SetUrl("antiforgery.js");

            // TODO: Why these are depended on AntiForgery?
            manifest.DefineScript("ko.datetime")
                .SetVersion("1.2")
                .SetUrl("kodatetime.js")
                .SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineScript("ko.autocomplete")
                .SetVersion("1.2")
                .SetUrl("autocomplete.js")
                .SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineScript("SmartWalk.ViewModels")
                .SetUrl("viewmodels.js")
                .SetDependencies("ko.datetime", "ko.autocomplete");

            manifest.DefineStyle("TextCollapse")
                .SetVersion("1.0")
                .SetUrl("text-collapse.css");

            manifest.DefineScript("TextCollapse")
                .SetVersion("1.0")
                .SetUrl("text-collapse.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("ImageScale")
                .SetVersion("1.3.1")
                .SetUrl("image-scale.min.js", "image-scale.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("AddThisEvent")
                .SetCdn("http://js.addthisevent.com/atemay.js")
                .SetDependencies("jQuery");
        }
    }
}