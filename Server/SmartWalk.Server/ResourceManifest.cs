using System.Linq;
using System.Web.Mvc;
using SmartWalk.Server.Common.Utils;
using SmartWalk.Server.Providers;
using SmartWalk.Shared;
using Orchard.UI.Resources;

namespace SmartWalk.Server
{
    [UsedImplicitly]
    public class ResourceManifest : IResourceManifestProvider
    {
        public ResourceManifest()
        {
            // TODO: This is temporary location for this code. We need to move to Application_Start
            ValueProviderFactories.Factories.Remove(
                ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonDotNetValueProviderFactory());
        }

        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // KnockoutJS

            manifest.DefineScript("ko")
                .SetVersion("3.1.0")
                .SetVersionUrl("knockout-3.1.0.js", "knockout-3.1.0.debug.js");

            manifest.DefineScript("ko.validation")
                .SetVersion("1.0.2")
                .SetVersionUrl("knockout.validation.js")
                .SetDependencies("ko");

            manifest.DefineScript("ko.autocomplete")
                .SetVersion("0.2.1")
                .SetVersionUrl("knockout-jqAutocomplete.min.js", "knockout-jqAutocomplete.js")
                .SetDependencies("ko", "jQueryUI");

            // 3rd Party - jQuery Visible

            manifest.DefineScript("jquery.visible")
                .SetVersion("1.2.0")
                .SetVersionUrl("jquery.visible.min.js", "jquery.visible.js")
                .SetDependencies("jQuery");

            // 3rd Party - Moment

            manifest.DefineScript("Moment")
                .SetVersion("2.7.0")
                .SetVersionUrl("moment.min.js", "moment.js");

            // SmartWalk

            // TODO: To setup *.min.css
            manifest.DefineStyle("SmartWalk")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw.css")
                .SetDependencies("SmartWalk.Theme");

            manifest.DefineScript("ko.datetime")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-ko-datetime.js")
                .SetDependencies("ko", "jQueryUI", "Moment");

            manifest.DefineScript("ko.switcher")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-ko-switcher.js")
                .SetDependencies("ko", "jQuery");

            manifest.DefineScript("SmartWalk.AntiForgery")
                .SetVersion("1.1")
                .SetVersionUrl("sw-antiforgery.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("SmartWalk.Common")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-common.js")
                .SetDependencies("jQuery", "ko", "jquery.visible", "Moment");

            manifest.DefineScript("SmartWalk.Editing")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-editing.js")
                .SetDependencies("SmartWalk.Common", "jQueryUI", "ko.validation",
                    // TODO: Maybe to fully relocate validation init into 3rd party components files
                    "ko.autocomplete", "ko.datetime", "ko.switcher");

            manifest.DefineScript("SmartWalk.Editing.Entity")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-editing-entity.js")
                .SetDependencies("SmartWalk.Editing");

            manifest.DefineScript("SmartWalk.Editing.Event")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-editing-event.js")
                .SetDependencies("SmartWalk.Editing");

            // 3rd Party - Text Collapse

            manifest.DefineStyle("TextCollapse")
                .SetVersion("1.0")
                .SetVersionUrl("text-collapse.css");

            manifest.DefineScript("TextCollapse")
                .SetVersion("1.0")
                .SetVersionUrl("text-collapse.js")
                .SetDependencies("jQuery");

            // 3rd Party - Image Scale

            manifest.DefineScript("ImageScale")
                .SetVersion("1.3.1")
                .SetVersionUrl("image-scale.min.js", "image-scale.js")
                .SetDependencies("jQuery");

            // 3rd Party - Add This Event

            manifest.DefineScript("AddThisEvent")
                .SetCdn("http://js.addthisevent.com/atemay.js")
                .SetDependencies("jQuery");
        }
    }
}