using System.Web.Mvc;

namespace SmartWalk.Server.Theme.Utils
{
    /// <summary>
    /// HACK: This is a wrapper class over Common.Utils.HtmlUtil one. 
    /// By some reason cshtml pages can't refer code in SmartWalk.Server.Common library.
    /// TODO: To figure out how to make the proper reference of code working
    /// </summary>
    public static class HtmlUtil
    {
        public static MvcHtmlString HelpBlockForValidation(MvcHtmlString validationText, object htmlAttributes)
        {
            return Common.Utils.HtmlUtil.HelpBlockForValidation(validationText, htmlAttributes);
        }
    }
}
