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

            manifest.DefineScript("JQuery-1-10-2").SetUrl("jquery-1.10.2.js");

            manifest.DefineScript("SmartWalk.Common").SetUrl("smartwalk-common.js").SetDependencies("jQuery");
            manifest.DefineScript("SmartWalk.AntiForgery").SetUrl("antiforgery.js");

            // TODO: Why these are depended on AntiForgery?
            manifest.DefineScript("ko.datetime").SetUrl("kodatetime.js").SetVersion("1.2").SetDependencies("ko", "SmartWalk.AntiForgery");
            manifest.DefineScript("ko.autocomplete").SetUrl("autocomplete.js").SetVersion("1.2").SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineScript("SmartWalk.ViewModels").SetUrl("viewmodels.js").SetVersion("1.8").SetDependencies("ko.datetime", "ko.autocomplete");

            manifest.DefineScript("ImageScale").SetUrl("image-scale.min.js").SetDependencies("jQuery");
            manifest.DefineScript("AddThisEvent").SetUrl("http://js.addthisevent.com/atemay.js").SetDependencies("jQuery");
        }
    }
}