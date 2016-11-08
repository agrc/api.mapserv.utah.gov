using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using WebAPI.Dashboard.Models.ViewModels;

namespace WebAPI.Dashboard.Extensions.Html
{
    public static class MessageBoxExtension
    {
        /// <summary>
        ///     Render an error box
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static MvcHtmlString ErrorBox(this HtmlHelper htmlHelper)
        {
            var errorViewData = htmlHelper.ViewData.Model as IErrorMessageDisplayable;

            if (errorViewData == null)
            {
                return MvcHtmlString.Empty;
            }

            if (errorViewData.ErrorMessage == null)
            {
                return MvcHtmlString.Empty;
            }

            var writer = new HtmlTextWriter(new StringWriter());

            writer.AddAttribute("class", "alert alert-danger text-center");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(errorViewData.ErrorMessage);
            writer.RenderEndTag();

            return MvcHtmlString.Create(writer.InnerWriter.ToString());
        }

        /// <summary>
        ///     Creates the html for an info box.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns></returns>
        public static MvcHtmlString MessageBox(this HtmlHelper htmlHelper)
        {
            var messageViewData = htmlHelper.ViewData.Model as IMessageDisplayable;

            if (messageViewData == null)
            {
                return MvcHtmlString.Empty;
            }

            if (messageViewData.Message == null)
            {
                return MvcHtmlString.Empty;
            }

            var writer = new HtmlTextWriter(new StringWriter());

            writer.AddAttribute("class", "alert alert-success text-center");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(messageViewData.Message);
            writer.RenderEndTag();

            return MvcHtmlString.Create(writer.InnerWriter.ToString());
        }
    }
}