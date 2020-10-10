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
    public class UserProductLotsController : Controller
    {
        private readonly AuctionContext _context;
        private readonly UserManager<AuctionUser> _userManager;

        public UserProductLotsController(AuctionContext context, UserManager<AuctionUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserProductLots
        public async Task<IActionResult> Index()
        {
            var auctionContext1 = _context.ProductLot.Include(p => p.Customer).Include(p => p.Owner)
                .Include(p => p.Product).Where(p => p.IsActive);
            return View(await auctionContext1.ToListAsync());
        }

        // GET: UserProductLots/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: UserProductLots/Create
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(User);
            ViewData["ProductId"] =
                new SelectList(
                    _context.Set<Product>().Include(p => p.ProductLots).AsEnumerable().Where(p =>
                        p.AuctionUserId == userId && p.CurrentLot == null),
                    "ID", "ProductName");
            return View();
        }

        // POST: UserProductLots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "ID,LotName,StartPrice,StartTrading,EndTrading,ProductId")]
            ProductLot productLot)
        {
            if (ModelState.IsValid)
            {
                productLot.IsActive = true;
                productLot.CurrentPrice = productLot.StartPrice;
                productLot.UpdateDateTime = DateTime.Now;
                productLot.OwnerAuctionUserId = _userManager.GetUserId(User);
                productLot.CustomerAuctionUserId = _userManager.GetUserId(User);
                productLot.CustomerAuctionUserId = null;
                _context.Add(productLot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ProductDesc", productLot.ProductId);
            return View(productLot);
        }

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

        // POST: UserProductLots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productLot = await _context.ProductLot.Include(p => p.Product).FirstOrDefaultAsync(p => p.ID == id);
            //productLot.Product.ProductLot = null;
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