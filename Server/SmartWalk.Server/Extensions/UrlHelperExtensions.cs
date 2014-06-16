using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartWalk.Server.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Register(this UrlHelper urlHelper, string userName, string loginData)
        {
            return urlHelper.Action("Register", "Account", new { area = SmartWalkConstants.SmartWalkArea, UserName = userName, ExternalLoginData = loginData });
        }

        //public static string OpenAuthLogOn(this UrlHelper urlHelper, string returnUrl)
        //{
        //    return urlHelper.Action("ExternalLogOn", "Account", new { area = Constants.LocalArea, ReturnUrl = returnUrl });
        //}

        //public static string Register(this UrlHelper urlHelper, string userName, string loginData)
        //{
        //    return urlHelper.Action("Register", "Account", new { area = Constants.OrchardUsersArea, UserName = userName, ExternalLoginData = loginData });
        //}

        //public static string Referer(this UrlHelper urlHelper, HttpRequestBase httpRequestBase)
        //{
        //    if (httpRequestBase.UrlReferrer != null)
        //    {
        //        return httpRequestBase.UrlReferrer.ToString();
        //    }
        //    return "~/";
        //}

        //public static string RemoveProviderConfiguration(this UrlHelper urlHelper, int id)
        //{
        //    return urlHelper.Action("Remove", "Admin", new { area = Constants.LocalArea, Id = id });
        //}

        //public static string ProviderCreate(this UrlHelper urlHelper)
        //{
        //    return urlHelper.Action("CreateProvider", "Admin", new { area = Constants.LocalArea });
        //}
    }
}