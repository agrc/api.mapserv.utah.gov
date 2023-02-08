using System;
using System.Collections.Generic;
using System.Security.Claims;
using developer.mapserv.utah.gov.Areas.Secure.Models.Database;
using developer.mapserv.utah.gov.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace developer.mapserv.utah.gov.TagHelpers {
    public class ApiKeyRowTagHelper : TagHelper {
        public ApiKeyDTO Key { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            output.TagName = "tr";
            output.TagMode = TagMode.StartTagAndEndTag;

            var tableHtml = $@"<td>{Key.Key}{KeyLabel(Key)}</td>
			<td class='hidden-xs hidden-sm'>{Key.CreatedAtTicks.ToDate()}</td>
            <td class='hidden-xs hidden-sm'><span class='text-right'>{Key.Usage}</span></td>
			<td class='hidden-xs hidden-sm'>{Key.Type}</td>
			<td class='hidden-xs'>{Key.LastUsed}</td>
			<td class='hidden-xs'>{Key.Pattern}</td>
			<td>{Key.Notes}</td>
			<td>{BuildButton(Key)}</td>";

            output.Content.SetHtmlContent(tableHtml);
        }

        private static string KeyLabel(ApiKeyDTO key) {
            var badge = "";
            if (key.Elevated) {
                badge = "<span style=\"margin-top: 5px;\" class=\"pull-right label label-info\">elevated</span>";
            }

            if (key.Configuration == "Development") {
                badge += "<span style=\"margin-top: 5px;\" class=\"pull-right label label-default\">dev</span>";
            }

            return badge;
        }

        private static string BuildButton(ApiKeyDTO key) {
            var buttonCssClass = "warning";
            var statusIconCssClass = "pause";
            var statusIconText = "Deactivate";

            if (!key.Enabled) {
                buttonCssClass = "success";
                statusIconCssClass = "play";
                statusIconText = "Activate";
            }

            var buttons = $@"<div class='btn-group pull-right'>
                            <a href='#' class='btn btn-{buttonCssClass}'><i data-key='{key.Key}' class='glyphicon glyphicon-{statusIconCssClass}'>
                                </i> {statusIconText}</a><a href = '#' class='btn btn-danger' title='Delete Key'>
                                <i data-key='{key.Key}' class='glyphicon glyphicon-trash'></i></a></div>";

            return buttons;
        }
    }
}
