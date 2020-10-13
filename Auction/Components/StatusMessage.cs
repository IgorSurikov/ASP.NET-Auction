using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Auction.Components
{
    [ViewComponent]
    public class StatusMessage
    {
        public IViewComponentResult Invoke(string statusMessage)

        {
            if (String.IsNullOrEmpty(statusMessage))
            {
                return new HtmlContentViewComponentResult(new HtmlString($""));
            }

            var statusMessageClass = statusMessage.StartsWith("Error") ? "danger" : "success";
            return new HtmlContentViewComponentResult(
                new HtmlString(
                    $" <div class=\"alert alert-{statusMessageClass} alert-dismissible\" role=\"alert\">" +
                    $"<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                    $"{statusMessage}" +
                    $"</div>")
            );

        }
    }
}