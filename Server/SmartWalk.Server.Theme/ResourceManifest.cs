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
                .SetVersion("2.1.4")
                .SetVersionUrl("jquery-2.1.4.min.js", "jquery-2.1.4.js")
                .SetCdn(
                    "//code.jquery.com/jquery-2.1.4.min.js",
                    "//code.jquery.com/jquery-2.1.4.js",
                    true);

            manifest.DefineScript("jQueryUI")
                .SetVersion("1.11.2")
                .SetVersionUrl("jquery-ui.min.js", "jquery-ui.js")
                .SetDependencies("jQuery")
                .SetCdn(
                    "//code.jquery.com/ui/1.11.2/jquery-ui.min.js",
                    "//code.jquery.com/ui/1.11.2/jquery-ui.js", 
                    true);

            manifest
                .DefineStyle("jQueryUI")
                .SetVersion("1.11.2")
                .SetVersionUrl("jquery-ui.min.css", "jquery-ui.css");

            #region jQueryUI is not used

            manifest
                .DefineStyle("jQueryUI.Structure")
                .SetVersion("1.11.2")
                .SetVersionUrl("jquery-ui.structure.min.css", "jquery-ui.structure.css")
                .SetDependencies("jQueryUI");

            manifest
                .DefineStyle("jQueryUI.Theme")
                .SetVersion("1.11.2")
                .SetVersionUrl("jquery-ui.theme.min.css", "jquery-ui.theme.css")
                .SetDependencies("jQueryUI.Structure", "jQueryUI");

            #endregion

            manifest.DefineScript("jQueryUI.SliderAccess")
                .SetVersion("0.3")
                .SetVersionUrl("jquery-ui-sliderAccess.min.js", "jquery-ui-sliderAccess.js")
                .SetDependencies("jQueryUI");

            manifest.DefineScript("jQueryUI.TimePicker")
                .SetVersion("1.5.0")
                .SetVersionUrl("jquery-ui-timepicker-addon.min.js", "jquery-ui-timepicker-addon.js")
                .SetDependencies("jQueryUI", "jQueryUI.SliderAccess");

            manifest.DefineStyle("jQueryUI.TimePicker")
                .SetVersion("1.5.0")
                .SetVersionUrl("jquery-ui-timepicker-addon.min.css", "jquery-ui-timepicker-addon.css")
                .SetDependencies("jQueryUI");

            manifest.DefineScript("jQuery.Easing")
                .SetVersion("1.3.0")
                .SetVersionUrl("jquery.easing.1.3.min.js", "jquery.easing.1.3.js")
                .SetDependencies("jQuery");

            manifest.DefineScript("jQuery.FitText")
                .SetVersion("1.2.0")
                .SetVersionUrl("jquery.fittext.min.js", "jquery.fittext.js")
                .SetDependencies("jQuery");

            // Bootstrap

            manifest.DefineStyle("Bootstrap")
                .SetVersion("3.3.1")
                .SetVersionUrl("bootstrap.min.css", "bootstrap.css")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css");

            manifest.DefineStyle("Bootstrap.Theme")
                .SetVersion("3.3.1")
                .SetVersionUrl("bootstrap-theme.min.css", "bootstrap-theme.css")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap-theme.min.css")
                .SetDependencies("Bootstrap");

            manifest.DefineScript("Bootstrap")
                .SetVersion("3.3.1")
                .SetVersionUrl("bootstrap.min.js", "bootstrap.js")
                .SetCdn("//netdna.bootstrapcdn.com/bootstrap/3.3.1/js/bootstrap.min.js")
                .SetDependencies("jQuery");

            // Animate

            manifest.DefineStyle("Animate")
                .SetVersion("3.3.0")
                .SetVersionUrl("../Content/animate.min.css", "../Content/animate.css");

            // Creative

            manifest.DefineStyle("Creative.Theme")
                .SetVersion("1.0.1")
                .SetVersionUrl("creative.min.css", "creative.css")
                .SetDependencies("Bootstrap", "Animate");

            manifest.DefineStyle("Creative.Theme.Override")
                .SetVersion("1.0.1")
                .SetVersionUrl("creative-override.min.css", "creative-override.css")
                .SetDependencies("Creative.Theme");

            manifest.DefineScript("WOW")
                .SetVersion("1.1.2")
                .SetVersionUrl("wow.min.js", "wow.js");

            manifest.DefineScript("Creative.Theme")
                .SetVersion("1.0.1")
                .SetVersionUrl("creative.min.js", "creative.js")
                .SetDependencies("jQuery", "jQuery.Easing", "jQuery.FitText", "WOW", "Bootstrap");

            // SmartWalk

            manifest.DefineStyle("SmartWalk.Theme")
                .SetVersion(VersionUtil.CurrentVersion)
                .SetVersionUrl("sw-theme.min.css", "sw-theme.css")
                .SetDependencies("Creative.Theme.Override");
        }
    }
}