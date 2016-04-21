using System.Collections.Generic;
using System.Web.Mvc;

namespace WebAPI.Dashboard.Extensions.Html
{
    public static class HtmlButtonExtension
    {
        public static MvcHtmlString Button(this HtmlHelper helper, string text,
                                           object htmlAttributes)
        {
            var builder = new TagBuilder("button")
            {
                InnerHtml = text
            };

            var attributes = (IDictionary<string, object>) HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            builder.MergeAttributes(attributes);

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}