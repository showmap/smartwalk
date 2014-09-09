using System.IO;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.UI.Resources;

namespace SmartWalk.Server.Common.Utils
{
    public static class HtmlUtil
    {
        public static MvcHtmlString HelpBlockForValidation(MvcHtmlString validationText, object htmlAttributes)
        {
            if (validationText != null)
            {
                var tagBuilder = new TagBuilder("span");
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                tagBuilder.InnerHtml = validationText.ToHtmlString();

                return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
            }

            return MvcHtmlString.Empty;
        }

        public static HtmlString Resource(ResourceDefinition resource, WorkContext context)
        {
            var defaultSettings = new RequireSettings
                {
                    DebugMode = context.HttpContext.IsDebuggingEnabled,
                    Culture = context.CurrentCulture,
                };

            var appPath = context.HttpContext.Request.ApplicationPath;
            var url = resource.ResolveUrl(defaultSettings, appPath);
            return new HtmlString(File.ReadAllText(context.HttpContext.Server.MapPath(url)));
        }
    }
}