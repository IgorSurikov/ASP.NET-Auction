using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
	public class MiniGameController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
