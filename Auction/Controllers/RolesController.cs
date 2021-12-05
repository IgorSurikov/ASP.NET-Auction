using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;
using Auction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auction.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AuctionUser> _userManager;
        private readonly SignInManager<AuctionUser> _signInManager;
        private readonly ILogger<AuctionProductLotsController> _logger;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AuctionUser> userManager, SignInManager<AuctionUser> signInManager, ILogger<AuctionProductLotsController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());

        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }

        public IActionResult UserList() => View(_userManager.Users.ToList());

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);

                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

        public async Task<IActionResult> Ban(string userId)
        {
            var current_user = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _userManager.FindByIdAsync(userId);
	        if (user == null) return NotFound();

            if (user.IsBanned)
            {
	            user.IsBanned = false;
	            _logger.Log(LogLevel.Information, "User {0} unban user {1}", current_user.FullName, user.FullName);
            }
            else
            {
	            user.IsBanned = true;
	            _logger.Log(LogLevel.Information, "User {0} ban user {1}", current_user.FullName, user.FullName);
            }
            await _userManager.UpdateAsync(user);

	        return RedirectToAction("UserList");


        }
    }
}
