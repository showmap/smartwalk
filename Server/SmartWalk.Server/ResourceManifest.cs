namespace SmartWalk.Server
{
    using Orchard.UI.Resources;

    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            

            manifest.DefineStyle("SmartWalk.AddPlace").SetUrl("add-place.css");
            manifest.DefineStyle("SmartWalk.AddEntity").SetUrl("add-entity.css");
            manifest.DefineStyle("SmartWalk.ListEvent").SetUrl("list-event.css");
            manifest.DefineStyle("SmartWalk.EditEvent").SetUrl("edit-event.css");           

            #region Google Maps
            manifest.DefineStyle("SmartWalk.GoogleMaps").SetUrl("google-maps.css");
            manifest.DefineScript("SmartWalk.GoogleMaps").SetUrl("https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places");
            #endregion

            #region Foursquare
            manifest.DefineStyle("FSquare.Autocomplete").SetUrl("fsquare-autocomplete.css");
            manifest.DefineScript("FSquare.Autocomplete").SetUrl("4sqacplugin.js");
            #endregion

            #region Mappy
            manifest.DefineStyle("Mappy").SetUrl("mappy.css");
            manifest.DefineScript("Mappy").SetUrl("mappy.js").SetDependencies("Bootstrap");
            #endregion
            
            #region SmartWalk
            manifest.DefineScript("SmartWalk.AntiForgery").SetUrl("antiforgery.js");

            manifest.DefineScript("ko.datetime").SetUrl("kodatetime.js").SetVersion("1.0").SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineScript("Bootstrap").SetUrl("bootstrap.js");
            manifest.DefineScript("JQuery-1-10-2").SetUrl("jquery-1.10.2.js");

            #region Host
            manifest.DefineStyle("SmartWalk.Host").SetUrl("host.css");
            manifest.DefineScript("SmartWalk.Entity").SetUrl("entity.js").SetVersion("1.1").SetDependencies("ko.datetime");
            manifest.DefineScript("SmartWalk.ViewModels").SetUrl("viewmodels.js").SetVersion("1.7").SetDependencies("ko.datetime");
            #endregion

            #endregion
        }
    }
}