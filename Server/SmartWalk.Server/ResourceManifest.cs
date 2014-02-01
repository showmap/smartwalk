namespace SmartWalk.Server
{
    using Orchard.UI.Resources;

    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineStyle("SmartWalk.AddPlaces").SetUrl("add-places.css");

            manifest.DefineStyle("SmartWalk.GoogleMaps").SetUrl("google-maps.css");
            manifest.DefineScript("SmartWalk.GoogleMaps").SetUrl("https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false&libraries=places");

            manifest.DefineStyle("FSquare.Autocomplete").SetUrl("fsquare-autocomplete.css");
            manifest.DefineScript("FSquare.Autocomplete").SetUrl("4sqacplugin.js");
        }
    }
}