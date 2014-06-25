using SmartWalk.Server.Common.Utils;
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

            // TODO: To setup *.min.css
            manifest.DefineStyle("SmartWalk")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk.css");

            manifest.DefineScript("SmartWalk.AntiForgery")
                .SetVersion("1.0")
                .SetVersionUrl("antiforgery.js");

            manifest.DefineScript("ko")
                .SetVersion("3.1.0")
                .SetVersionUrl("knockout-3.1.0.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("ko.validation")
                .SetVersion("1.0.2")
                .SetVersionUrl("knockout-validation.js")
                .SetDependencies("ko");

            manifest.DefineScript("SmartWalk.Utilites")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-utilites.js")
                .SetDependencies("ko", "ko.validation");            

            manifest.DefineScript("ko.datetime")
                .SetVersion("1.2")
                .SetVersionUrl("knockout-datetime.js")
                .SetDependencies("ko", "SmartWalk.Utilites");

            manifest.DefineScript("ko.switcher")
                .SetVersion("1.2")
                .SetVersionUrl("knockout-switcher.js")
                .SetDependencies("ko", "SmartWalk.Utilites");

            manifest.DefineScript("ko.autocomplete")
                .SetVersion("1.1")
                .SetVersionUrl("knockout-autocomplete.js")
                .SetDependencies("ko", "SmartWalk.AntiForgery", "SmartWalk.Utilites");            
            
            manifest.DefineScript("SmartWalk.ViewModels.Common")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-viewmodels-common.js")
                .SetDependencies(
                    "ko.validation", 
                    "ko.datetime", 
                    "ko.switcher", 
                    "ko.autocomplete", 
                    "SmartWalk.Utilites");

            manifest.DefineScript("SmartWalk.ViewModels.Entity")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-viewmodels-entity.js")
                .SetDependencies("SmartWalk.ViewModels.Common");

            manifest.DefineScript("SmartWalk.ViewModels.Entity.Extended")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-viewmodels-entity-extended.js")
                .SetDependencies("SmartWalk.ViewModels.Entity");

            manifest.DefineScript("SmartWalk.ViewModels.Event")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-viewmodels-event.js")
                .SetDependencies("SmartWalk.ViewModels.Entity");

            manifest.DefineScript("SmartWalk.ViewModels.Event.Extended")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("smartwalk-viewmodels-event-extended.js")
                .SetDependencies("SmartWalk.ViewModels.Event");

            manifest.DefineStyle("TextCollapse")
                .SetVersion("1.0")
                .SetVersionUrl("text-collapse.css");

            manifest.DefineScript("TextCollapse")
                .SetVersion("1.0")
                .SetVersionUrl("text-collapse.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("ImageScale")
                .SetVersion("1.3.1")
                .SetVersionUrl("image-scale.min.js", "image-scale.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("AddThisEvent")
                .SetCdn("http://js.addthisevent.com/atemay.js")
                .SetDependencies("jQuery");
        }
    }
}