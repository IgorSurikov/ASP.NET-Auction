using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Auction.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "asp-active")]
    [HtmlTargetElement("tr", Attributes = "asp-active")]
    public class ItemTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-active")] public bool Active { get; set; }


        public override async void Process(TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder builder = new TagBuilder(output.TagName);

            if (Active)
            {
                builder.AddCssClass("table-dark");
                string content = (await output.GetChildContentAsync()).GetContent();
                //output.Content.SetContent($"{content}");
                content = Regex.Replace(content, "(<a.+<\\/a> \\|\r\n\\s+)+(<a.+<\\/a>)", "Not available");
                output.Content.SetHtmlContent(content);
            }

            output.MergeAttributes(builder);
        }
    }
}