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
using Microsoft.Extensions.Logging;

namespace Auction.Controllers
{
    public class UserProductLotsController : Controller
    {
        private readonly AuctionContext _context;
        private readonly UserManager<AuctionUser> _userManager;
        private readonly ILogger<UserProductLotsController> _logger;

        public UserProductLotsController(AuctionContext context, UserManager<AuctionUser> userManager, ILogger<UserProductLotsController> _logger)
        {
            _context = context;
            _userManager = userManager;
            this._logger = _logger;
        }

        // GET: UserProductLots
        public async Task<IActionResult> Index()
        {
            var auctionContext = _context.ProductLot.Include(p => p.Customer)
                .Include(p => p.Owner)
                .Include(p => p.Product)
                .Where(p => p.IsActive && p.OwnerAuctionUserId == _userManager.GetUserId(User));
            return View(await auctionContext.ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "ID,LotName,StartPrice,StartTrading,EndTrading,ProductId")]
            ProductLot productLot)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ProductDesc", productLot.ProductId);
                return View(productLot);
            }

            if (ProductLotExists(productLot.LotName))
            {
                ViewData["StatusMessage"] = "Error: this lot name already exists";
                ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ProductDesc", productLot.ProductId);
                return View(productLot);
            }
            productLot.IsActive = true;
            productLot.CurrentPrice = productLot.StartPrice;
            productLot.UpdateDateTime = DateTime.Now;
            productLot.OwnerAuctionUserId = _userManager.GetUserId(User);
            productLot.CustomerAuctionUserId = _userManager.GetUserId(User);
            _context.Add(productLot);
            await _context.SaveChangesAsync();
            _logger.Log(LogLevel.Information, "User {0} create Lot: {1}", User?.Identity?.Name, productLot.LotName);
            return RedirectToAction(nameof(Index));

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
            _logger.Log(LogLevel.Information, "User {0} delete Lot: {1}", User?.Identity?.Name, productLot.LotName);
            return View(productLot);
        }

        // POST: UserProductLots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productLot = await _context.ProductLot.Include(p => p.Product).FirstOrDefaultAsync(p => p.ID == id);
            var productLots = _context.ProductLot.Include(p => p.Product)
                .Where(p => p.LotName == productLot.LotName);
            _context.ProductLot.RemoveRange(productLots);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductLotExists(string lotName)
        {
            return _context.ProductLot.Any(e => e.LotName == lotName);
        }
    }
}