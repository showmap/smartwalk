using SmartWalk.Server.Common;
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

            // KnockoutJS

            manifest.DefineScript("ko")
                .SetVersion("3.3.0")
                .SetVersionUrl("knockout-3.3.0.js", "knockout-3.3.0.debug.js");

            manifest.DefineScript("ko.validation")
                .SetVersion("1.0.2")
                .SetVersionUrl("knockout.validation.min.js", "knockout.validation.js")
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
                .SetVersion("2.10.6")
                .SetVersionUrl("moment.min.js", "moment.js");

            // 3rd Party - JsHashtable

            manifest.DefineScript("JsHashtable")
                .SetVersion("3.0")
                .SetVersionUrl("hashtable.min.js", "hashtable.js");

            manifest.DefineScript("JsHashset")
                .SetVersion("3.0")
                .SetVersionUrl("hashset.min.js", "hashset.js")
                .SetDependencies("JsHashtable");

            // 3rd Party - JQuery-File-Upload

            manifest.DefineStyle("jquery.fileupload")
                .SetVersion("1.3.0")
                .SetVersionUrl("jquery.fileupload.min.css", "jquery.fileupload.css");

            manifest.DefineScript("jquery.iframe-transport")
                .SetVersion("1.8.2")
                .SetVersionUrl("jquery.iframe-transport.min.js", "jquery.iframe-transport.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("jquery.fileupload")
                .SetVersion("5.42.0")
                .SetVersionUrl("jquery.fileupload.min.js", "jquery.fileupload.js")
                .SetDependencies("jQueryUI");

            // 3rd Party - Leaflet

            /*manifest.DefineScript("Leaflet")
                .SetVersion("0.7.3")
                .SetVersionUrl("leaflet.js", "leaflet-src.js");

            manifest.DefineStyle("Leaflet")
                .SetVersion("0.7.3")
                .SetVersionUrl("leaflet.css");*/

            // 3rd Party - Gogle Maps API
            
            manifest.DefineScript("GoogleMapsApi")
                .SetCdn(
                    "http://maps.google.com/maps/api/js?v=3.exp&key=" + Settings.GoogleMapsKey,
                    "http://maps.google.com/maps/api/js?v=3.exp");

            // SmartWalk

            manifest.DefineStyle("SmartWalk")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw.min.css", "sw.css")
                .SetDependencies("SmartWalk.Theme");

            manifest.DefineScript("ko.datetime")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-ko-datetime.min.js", "sw-ko-datetime.js")
                .SetDependencies("ko", "jQueryUI", "Moment");

            manifest.DefineScript("ko.mappicker")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-ko-mappicker.min.js", "sw-ko-mappicker.js")
                .SetDependencies("ko", "jQueryUI"); // on window loaded "GoogleMapsApi"

            manifest.DefineScript("ko.switcher")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-ko-switcher.min.js", "sw-ko-switcher.js")
                .SetDependencies("ko", "jQuery");

            manifest.DefineScript("SmartWalk.AntiForgery")
                .SetVersion("1.1")
                .SetVersionUrl("sw-antiforgery.min.js", "sw-antiforgery.js")
                .SetDependencies("jQuery");

            // This resource is included directly into content to run before page render
            manifest.DefineScript("SmartWalk.List.Initial")
                .SetUrl("sw-list-initial.min.js", "sw-list-initial.js");

            manifest.DefineScript("SmartWalk.Common")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-common.min.js", "sw-common.js")
                .SetDependencies("jQuery", "ko", "jquery.visible", "Moment");

            manifest.DefineScript("SmartWalk.Editing")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-editing.min.js", "sw-editing.js")
                .SetDependencies("SmartWalk.Common", "jQueryUI", "JsHashset", "ko.validation", "jquery.fileupload",
                    // TODO: Maybe to fully relocate validation init into 3rd party components files
                    "ko.autocomplete", "ko.datetime", "ko.switcher", "ko.mappicker");

            manifest.DefineScript("SmartWalk.Editing.Entity")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-editing-entity.min.js", "sw-editing-entity.js")
                .SetDependencies("SmartWalk.Editing"); // on window loaded "GoogleMapsApi"

            manifest.DefineScript("SmartWalk.Editing.Event")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-editing-event.min.js", "sw-editing-event.js")
                .SetDependencies("SmartWalk.Editing");

            // 3rd Party - Text Collapse

            manifest.DefineStyle("TextCollapse")
                .SetVersion("1.0")
                .SetVersionUrl("text-collapse.min.css", "text-collapse.css");

            manifest.DefineScript("TextCollapse")
                .SetVersion("1.0")
                .SetVersionUrl("text-collapse.min.js", "text-collapse.js")
                .SetDependencies("jQuery");

            // 3rd Party - Image Scale

            manifest.DefineScript("ImageScale")
                .SetVersion("1.3.2")
                .SetVersionUrl("image-scale.min.js", "image-scale.js")
                .SetDependencies("jQuery");

            // 3rd Party - Add This Event

            manifest.DefineScript("AddThisEvent")
                .SetCdn("http://addthisevent.com/libs/1.6.0/ate.min.js");

            manifest.DefineStyle("AddThisEvent")
                .SetVersion("1.6")
                .SetVersionUrl("addthisevent.theme2.min.css", "addthisevent.theme2.css");
        }
    }
}