using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auction.Data;
using Auction.Models;
using Auction.Areas.Identity.Data;
using Auction.Signals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Logging;

namespace Auction.Controllers
{
    public class AuctionProductLotsController : Controller
    {
        private readonly AuctionContext _context;
        private readonly UserManager<AuctionUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<AuctionProductLotsController> _logger;

        public AuctionProductLotsController(AuctionContext context, UserManager<AuctionUser> userManager, IHubContext<NotificationHub> hubContext, ILogger<AuctionProductLotsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _logger = logger;
        }

        // GET: AuctionProductLots
        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.Now;
            var auctionContext = _context.ProductLot
                .Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product)
                .Where(p => p.IsActive && p.StartTrading <= currentDate && p.EndTrading > currentDate);

            var auctionTransactions = _context.ProductLot
                .Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product)
                .Where(p => p.IsActive && p.EndTrading <= currentDate);
            foreach (var t in auctionTransactions)
            {
                if (t.OwnerAuctionUserId == t.CustomerAuctionUserId)
                {
                    _context.ProductLot.Remove(t);
                    _logger.Log(LogLevel.Information, "the Lot: {0} has expired", t.LotName);
                }
                else
                {
                    var transaction = new Transaction()
                    {
                        ProductId = t.ProductId,
                        OwnerAuctionUserId = t.OwnerAuctionUserId,
                        CustomerAuctionUserId = t.CustomerAuctionUserId,
                        TransactionAmount = t.CurrentPrice,
                        InsertDateTime = DateTime.Now
                    };
                    t.IsActive = false;
                    t.Owner.Wallet += t.CurrentPrice;
                    t.Product.AuctionUser = t.Customer;
                    _context.Add(transaction);
                    _context.ProductLot.RemoveRange(_context.ProductLot.Where(p => p.LotName == t.LotName));
                    _logger.Log(LogLevel.Information, "the Lot: {0} was sold", t.LotName);
                }

                _context.Update(t);
            }


            await _context.SaveChangesAsync();
            return View(await auctionContext.ToListAsync());
        }

        // GET: AuctionProductLots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            

            var auctionContext = _context.ProductLot.Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product);
            string lotName = auctionContext.FirstOrDefault(p => p.ID == id)?.LotName;
            _logger.Log(LogLevel.Information, "User {0} check history of Lot: {1}", User?.Identity?.Name, lotName);
            return View(await auctionContext
                .Where(p => p.LotName == lotName)
                .OrderBy(p => p.UpdateDateTime)
                .ToListAsync());
        }


        public async Task<IActionResult> RaisePrice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productLot = await _context.ProductLot.Include(p => p.Product).FirstOrDefaultAsync(p => p.ID == id);
            if (productLot == null)
            {
                return NotFound();
            }

            return View(productLot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RaisePrice(int id, int newPrice)
        {
            var productLot = await _context.ProductLot.Include(p => p.Product).FirstOrDefaultAsync(p => p.ID == id);
            if (productLot == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (newPrice >= user.Wallet)
            {
	            _logger.Log(LogLevel.Error, "User {0} doesn't have enough money. lot: {1}, price: {2} ", User?.Identity?.Name,productLot.LotName, newPrice);
                ViewData["StatusMessage"] = "Error: You don't have enough money.";
                return View(productLot);
            }

            if (newPrice <= productLot.CurrentPrice)
            {
	            _logger.Log(LogLevel.Error, "User {0} price less then current price. lot: {1}, price: {2} ", User?.Identity?.Name, productLot.LotName, newPrice);
                ViewData["StatusMessage"] = "Error: New price less then current price.";
                return View(productLot);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    productLot.IsActive = false;
                    _context.Update(productLot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductLotExists(productLot.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                user.Wallet -= (newPrice - productLot.CurrentPrice);
                var newProductLot = (ProductLot) productLot.Clone();
                newProductLot.ID = 0;
                newProductLot.CustomerAuctionUserId = _userManager.GetUserId(User);
                newProductLot.IsActive = true;
                newProductLot.CurrentPrice = newPrice;
                newProductLot.UpdateDateTime = DateTime.Now;
                _context.Update(user);
                _context.Add(newProductLot);
                await _context.SaveChangesAsync();
                _logger.Log(LogLevel.Information, "User {0} increased the lot price. lot: {1}, price: {2} ", User?.Identity?.Name, productLot.LotName, newPrice);
                ViewData["StatusMessage"] = "Congratulations, you have successfully increased price.";
                await _hubContext.Clients.User(newProductLot.OwnerAuctionUserId).SendAsync("Notify", $"Lot Name: {productLot.LotName}. User {user.FullName} increased the price for {newPrice - productLot.CurrentPrice} $");
                return View(newProductLot);
            }

            return View(productLot);
        }

        // GET: AuctionProductLots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lotName = _context.ProductLot.FirstOrDefault(m => m.ID == id)?.LotName;
            var lots = _context.ProductLot.Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product).Where(l => l.LotName == lotName).OrderByDescending(l => l.UpdateDateTime);
            var lotsList = lots.ToList();
            for (int i = 0; i < lotsList.Count - 1; i++)
            {
                var diff = lotsList[i].CurrentPrice - lotsList[i + 1].CurrentPrice;
                lotsList[i].Customer.Wallet += diff;
                _context.Update(lotsList[i].Customer);
            }
            _context.ProductLot.RemoveRange(lots);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }


        private bool ProductLotExists(int id)
        {
            return _context.ProductLot.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Cancel(string lotName)
        {
            if (lotName == null)
            {
                return NotFound();
            }
            var lots = _context.ProductLot.Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product).Where(l => l.LotName == lotName).OrderByDescending(l => l.UpdateDateTime);
            var lotsList = lots.ToList();
            var diff = lotsList[0].CurrentPrice - lotsList[1].CurrentPrice;
            lotsList[0].Customer.Wallet += diff;
            (await _context.ProductLot.FindAsync(lotsList[1].ID)).IsActive = true;
            _context.ProductLot.Remove((await _context.ProductLot.FindAsync(lotsList[0].ID)));
            await _context.SaveChangesAsync();
      
            return RedirectToAction(nameof(Details), new { id = lotsList[1].ID });
        }
    }
}