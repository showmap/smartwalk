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
            manifest.DefineStyle("Mappy").SetUrl("mappy.css");

            manifest.DefineStyle("SmartWalk.GoogleMaps").SetUrl("google-maps.css");
            manifest.DefineScript("SmartWalk.GoogleMaps").SetUrl("https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places");

        
            manifest.DefineScript("Bootstrap").SetUrl("bootstrap.js");
            manifest.DefineScript("JQuery-1-10-2").SetUrl("jquery-1.10.2.js");

            manifest.DefineScript("SmartWalk.AntiForgery").SetUrl("antiforgery.js");

            manifest.DefineScript("ko.datetime").SetUrl("kodatetime.js").SetDependencies("ko", "SmartWalk.AntiForgery");

            manifest.DefineStyle("FSquare.Autocomplete").SetUrl("fsquare-autocomplete.css");
            manifest.DefineScript("FSquare.Autocomplete").SetUrl("4sqacplugin.js");

            manifest.DefineScript("Mappy").SetUrl("mappy.js").SetDependencies("Bootstrap");
        }
    }
}