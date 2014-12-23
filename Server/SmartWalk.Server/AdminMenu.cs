using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using SmartWalk.Shared;

namespace SmartWalk.Server
{
    public class AdminMenu : INavigationProvider
    {
        [UsedImplicitly]
        public Localizer T { get; set; }

        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(item => item
                    .Caption(T("SmartWalk"))
                    .Position("20")
                    .Permission(StandardPermissions.SiteOwner)
                    .Add(subItem => subItem
                        .Caption(T("Info"))
                        .Action("Index", "Admin", new { area = "SmartWalk.Server" })
                        .Permission(StandardPermissions.SiteOwner)
                        .LocalNav())
                    .Add(subItem => subItem
                        .Caption(T("Import Images"))
                        .Action("ImportImages", "Admin", new { area = "SmartWalk.Server" })
                        .Permission(StandardPermissions.SiteOwner)
                        .LocalNav())
                    .Add(subItem => subItem
                        .Caption(T("Import XML Data"))
                        .Action("ImportXMLData", "Admin", new { area = "SmartWalk.Server" })
                        .Permission(StandardPermissions.SiteOwner)
                        .LocalNav()));
        }
    }
}