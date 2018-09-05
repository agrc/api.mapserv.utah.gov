using System.Threading.Tasks;
using developer.mapserv.utah.gov.Areas.Secure.Models.ViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace developer.mapserv.utah.gov.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        public bool Confirmed { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var content = await output.GetChildContentAsync();
            var email = content.GetContent();

            var labelCss = "success";
            var labelText = "Confirmed";
            var buttonGroupCss = "";
            var buttonGroup = "";

            if (!Confirmed)
            {
                labelCss = "danger";
                labelText = "Unconfirmed";
                buttonGroupCss = "class='input-group'";
                buttonGroup = @"<span class='input-group-btn'>
					<button id='confirm' class='btn btn-warning'>Confirm</button>
				</span>";
            }

            var template = $@"<label class='col-sm-4 control-label' for 'Email'>
        		<span class='label label-{labelCss}'>{labelText}</span> Email
  			</label>
            <div class='col-sm-8'>
                <div {buttonGroupCss}>
					<input type='email' data-val='true' id='Email' name='Email' value='{email}' class='form-control' />
                	{buttonGroup}
                </div>
            </div>";

            output.Content.SetHtmlContent(template);
        }
    }
}
