﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Settings;
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
                    DebugMode = context.CurrentSite.ResourceDebugMode == ResourceDebugMode.Enabled,
                    Culture = context.CurrentCulture,
                };

            var appPath = context.HttpContext.Request.ApplicationPath;
            var url = resource.ResolveUrl(defaultSettings, appPath);
            return new HtmlString(File.ReadAllText(context.HttpContext.Server.MapPath(url)));
        }

        public static string ResourceUrl(this ResourceDefinition resource, WorkContext context, bool cdnMode = false)
        {
            var result = resource != null
                ? resource.ResolveUrl(new RequireSettings
                    {
                        DebugMode = context.CurrentSite.ResourceDebugMode == ResourceDebugMode.Enabled,
                        CdnMode = cdnMode
                    }, null)
                : null;

            return result;
        }

        public static void SetiTunesMeta(this IOrchardViewPage page)
        {
            page.SetMeta(
                "apple-itunes-app",
                $"app-id={Settings.iTunesAppId}, app-argument={page.WorkContext.HttpContext.Request.Url}", 
                null, 
                null);
        }

        public static LocalizedString AdaptiveCapture(string caption, string extension)
        {
            var result = NullLocalizer.Instance("{0}{1} {2}{3}",
                caption,
                new HtmlString("<span class=\"hidden-xs\">"),
                extension,
                new HtmlString("</span>"));
            return result;
        }

        public static string ToCssClasses(this IEnumerable<string> classes)
        {
            var result = classes.Aggregate(string.Empty, 
                (s, i) => s + (s != string.Empty ? " " : string.Empty) + i);
            return result;
        }
    }
}