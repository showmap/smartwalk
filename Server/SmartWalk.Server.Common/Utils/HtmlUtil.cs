using System.Web.Mvc;

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
    }
}
