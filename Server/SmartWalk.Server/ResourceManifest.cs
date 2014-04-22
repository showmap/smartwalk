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

            #region Google Maps
            manifest.DefineStyle("SmartWalk.GoogleMaps").SetUrl("google-maps.css");
            manifest.DefineScript("SmartWalk.GoogleMaps").SetUrl("https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places");
            #endregion

            #region Foursquare
            manifest.DefineStyle("FSquare.Autocomplete").SetUrl("fsquare-autocomplete.css");
            manifest.DefineScript("FSquare.Autocomplete").SetUrl("4sqacplugin.js");
            #endregion

            #region Mappy
            //manifest.DefineStyle("Mappy").SetUrl("mappy.css");
            //manifest.DefineScript("Mappy").SetUrl("mappy.js").SetDependencies("Bootstrap");
            #endregion
            
            #region SmartWalk
            manifest.DefineScript("SmartWalk.AntiForgery").SetUrl("antiforgery.js");

            manifest.DefineScript("ko.datetime").SetUrl("kodatetime.js").SetVersion("1.2").SetDependencies("ko", "SmartWalk.AntiForgery");
            manifest.DefineScript("ko.autocomplete").SetUrl("autocomplete.js").SetVersion("1.2").SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineScript("JQuery-1-10-2").SetUrl("jquery-1.10.2.js");

            manifest.DefineScript("SmartWalk.ViewModels").SetUrl("viewmodels.js").SetVersion("1.8").SetDependencies("ko.datetime", "ko.autocomplete");
            #endregion

            manifest.DefineScript("ImageScale").SetUrl("image-scale.min.js").SetDependencies("jQuery");
            manifest.DefineScript("AddThisEvent").SetUrl("http://js.addthisevent.com/atemay.js").SetDependencies("jQuery");
        }
    }
}