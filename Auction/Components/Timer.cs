using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Auction.Components
{
    [ViewComponent]
    public class Timer
    {
        public IViewComponentResult Invoke()
        {
            return new HtmlContentViewComponentResult(
                new HtmlString($"<p>Current auction time:<b>{DateTime.Now.ToString("HH:mm:ss")}</b></p>")
            );

        }
    }
}