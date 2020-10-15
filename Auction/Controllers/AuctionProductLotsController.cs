using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Auction.Data;
using Auction.Models;
using Auction.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;

namespace Auction.Controllers
{
    public class AuctionProductLotsController : Controller
    {
        private readonly AuctionContext _context;
        private readonly UserManager<AuctionUser> _userManager;

        public AuctionProductLotsController(AuctionContext context, UserManager<AuctionUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .Where(p => p.IsActive && p.EndTrading <= currentDate &&
                            p.OwnerAuctionUserId != p.CustomerAuctionUserId);
            foreach (var t in auctionTransactions)
            {
                var transaction = new Transaction()
                {
                    ProductId = t.ProductId,
                    OwnerAuctionUserId = t.OwnerAuctionUserId,
                    CustomerAuctionUserId = t.CustomerAuctionUserId,
                    TransactionAmount = t.CurrentPrice
                };
                t.IsActive = false;
                t.Owner.Wallet += t.CurrentPrice;
                t.Product.AuctionUser = t.Customer;
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

            var auctionContext = _context.ProductLot.Include(p => p.Customer).Include(p => p.Owner)
                .Include(p => p.Product);
            string lotName = auctionContext.FirstOrDefault(p => p.ID == id)?.LotName;


            return View(await auctionContext.Where(p => p.LotName == lotName).OrderBy(p => p.UpdateDateTime)
                .ToListAsync());
        }

        // GET: AuctionProductLots/Create


        // GET: AuctionProductLots/Edit/5
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

        // POST: AuctionProductLots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                ViewData["StatusMessage"] = "Error: You don't have enough money.";
                return View(productLot);
            }

            if (newPrice <= productLot.CurrentPrice)
            {
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
                ViewData["StatusMessage"] = "Congratulations, you have successfully increased price.";
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

            var productLot = await _context.ProductLot
                .Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productLot == null)
            {
                return NotFound();
            }

            return View(productLot);
        }

        // POST: AuctionProductLots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productLot = await _context.ProductLot.FindAsync(id);
            _context.ProductLot.Remove(productLot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductLotExists(int id)
        {
            return _context.ProductLot.Any(e => e.ID == id);
        }
    }
}