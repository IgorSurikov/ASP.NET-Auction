using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ApiMiniGameController : ControllerBase
	{

		private readonly UserManager<AuctionUser> _userManager;
		public ApiMiniGameController(UserManager<AuctionUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpPost("{rating}")]
		public async Task<ActionResult<AuctionUser>> SaveRating(int rating)
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			if (currentUser == null) return NotFound();

			currentUser.Rating = rating;
			await _userManager.UpdateAsync(currentUser);
			return currentUser;

		}
	}


}
