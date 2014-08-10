using System.Web.Mvc;

namespace SmartWalk.Server.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string EditProfile(this UrlHelper urlHelper)
        {
            return urlHelper.Action("EditProfile", "Account", new { area = SmartWalkConstants.SmartWalkArea });
        }

        public static string ChangePassword(this UrlHelper urlHelper)
        {
            return urlHelper.Action("ChangePassword", "Account", new { area = SmartWalkConstants.OrchardUsersArea });
        }
    }
}