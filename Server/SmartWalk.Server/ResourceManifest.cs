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
                .SetUrl("ko-datetime.js")
                .SetDependencies("ko");

            manifest.DefineScript("ko.switcher")
                .SetVersion("1.2")
                .SetUrl("ko-switcher.js")
                .SetDependencies("ko");

            manifest.DefineScript("ko.autocomplete")
                .SetVersion("1.1")
                .SetUrl("ko-autocomplete.js")
                .SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineScript("SmartWalk.Utilites")
                .SetUrl("smartwalk-utilites.js?ver=1.0.0")
                .SetDependencies("ko");
            
            manifest.DefineScript("SmartWalk.ViewModels.Common")
                .SetUrl("smartwalk-viewmodels-common.js")
                .SetDependencies("ko.datetime", "ko.switcher", "ko.autocomplete", "SmartWalk.Utilites");

            manifest.DefineScript("SmartWalk.ViewModels.Entity")
                .SetUrl("smartwalk-viewmodels-entity.js?ver=1.1")
                .SetDependencies("SmartWalk.ViewModels.Common");

            manifest.DefineScript("SmartWalk.ViewModels.Entity.Extended")
                .SetUrl("smartwalk-viewmodels-entity-extended.js?ver=1.1")
                .SetDependencies("SmartWalk.ViewModels.Entity");

            manifest.DefineScript("SmartWalk.ViewModels.Event")
                .SetUrl("smartwalk-viewmodels-event.js?ver=1.5")
                .SetDependencies("SmartWalk.ViewModels.Entity");

            manifest.DefineScript("SmartWalk.ViewModels.Event.Extended")
                .SetUrl("smartwalk-viewmodels-event-extended.js?ver=1.4")
                .SetDependencies("SmartWalk.ViewModels.Event");

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