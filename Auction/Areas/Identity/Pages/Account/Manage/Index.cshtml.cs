using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auction.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<AuctionUser> _userManager;
        private readonly SignInManager<AuctionUser> _signInManager;

        public IndexModel(
            UserManager<AuctionUser> userManager,
            SignInManager<AuctionUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string FullName { get; set; }
        public string Wallet { get; set; }

        [TempData] public string StatusMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Amount of money")]
            public int MoneyAmount { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            FullName = user.FullName;
            Wallet = user.Wallet.ToString();

            Input = new InputModel
            {
                MoneyAmount = 0
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            var moneyAmount = Input.MoneyAmount;
            if (moneyAmount != 0)
            {
                user.Wallet += moneyAmount;
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}