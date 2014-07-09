using Orchard.UI.Resources;
using SmartWalk.Server.Common.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server.Theme
{
    [UsedImplicitly]
    public class ResourceManifest : IResourceManifestProvider
    {
        public static string GetCurrentVersion()
        {
            return VersionUtil.CurrentVersion;
        }

        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // jQuery

            manifest.DefineScript("jQuery")
                .SetVersion("2.1.1")
                .SetVersionUrl("jquery-2.1.1.min.js", "jquery-2.1.1.js")
                .SetCdn(
                    "//code.jquery.com/jquery-2.1.1.min.js",
                    "//code.jquery.com/jquery-2.1.1.js",
                    true);

            // HACK Using fake "9." prefix due to Orchard's isuse https://orchard.codeplex.com/workitem/20798
            const string jQueryUIVersion = "9.1.11.0";

            manifest.DefineScript("jQueryUI")
                .SetVersion(jQueryUIVersion)
                .SetVersionUrl("jquery-ui.min.js", "jquery-ui.js")
                .SetDependencies("jQuery")
                .SetCdn(
                    "//code.jquery.com/ui/1.11.0/jquery-ui.min.js",
                    "//code.jquery.com/ui/1.11.0/jquery-ui.js", 
                    true);

            manifest
                .DefineStyle("jQueryUI")
                .SetVersion(jQueryUIVersion)
                .SetVersionUrl("jquery-ui.min.css", "jquery-ui.css");

            #region jQueryUI is not used

            manifest
                .DefineStyle("jQueryUI.Structure")
                .SetVersion(jQueryUIVersion)
                .SetVersionUrl("jquery-ui.structure.min.css", "jquery-ui.structure.css")
                .SetDependencies("jQueryUI");

            manifest
                .DefineStyle("jQueryUI.Theme")
                .SetVersion(jQueryUIVersion)
                .SetVersionUrl("jquery-ui.theme.min.css", "jquery-ui.theme.css")
                .SetDependencies("jQueryUI.Structure", "jQueryUI");

            #endregion

            manifest.DefineScript("jQueryUI.SliderAccess")
                .SetVersion("0.3")
                .SetVersionUrl("jquery-ui-sliderAccess.js")
                .SetDependencies("jQueryUI");

            manifest.DefineScript("jQueryUI.TimePicker")
                .SetVersion("1.4.5")
                .SetVersionUrl("jquery-ui-timepicker-addon.min.js", "jquery-ui-timepicker-addon.js")
                .SetDependencies("jQueryUI", "jQueryUI.SliderAccess");

            manifest.DefineStyle("jQueryUI.TimePicker")
                .SetVersion("1.4.5")
                .SetVersionUrl("jquery-ui-timepicker-addon.min.css", "jquery-ui-timepicker-addon.css")
                .SetDependencies("jQueryUI");

            // Bootstrap

            manifest.DefineStyle("Bootstrap")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap.min.css", "bootstrap.css")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css");

            manifest.DefineStyle("Bootstrap.Theme")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap-theme.min.css", "bootstrap-theme.css")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css")
                .SetDependencies("Bootstrap");

            manifest.DefineScript("Bootstrap")
                .SetVersion("3.1.1")
                .SetVersionUrl("bootstrap.min.js", "bootstrap.js")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js")
                .SetDependencies("jQuery");

            // SmartWalk

            // TODO: To setup *.min.css
            manifest.DefineStyle("SmartWalk.Theme")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-theme.css")
                .SetDependencies("Bootstrap.Theme");
        }
    }
}