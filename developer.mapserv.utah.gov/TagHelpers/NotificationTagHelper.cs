using Microsoft.AspNetCore.Razor.TagHelpers;

namespace developer.mapserv.utah.gov.TagHelpers
{
    public class NotificationTagHelper : TagHelper
    {
        public string Error { get; set; }
        public string Message { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(Message))
            {
                // output.SuppressOutput();

                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var message = Message;
            var classes = "alert alert-success text-center";
            if (string.IsNullOrEmpty(Message))
            {
                message = Error;
                classes = "alert alert-danger text-center";
            }

            output.Attributes.Add("class", classes);
            output.Content.SetHtmlContent(message);
        }
    }
}
