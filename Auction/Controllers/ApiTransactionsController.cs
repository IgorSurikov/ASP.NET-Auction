using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.Data;
using Auction.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTransactionsController : ControllerBase
    {
        private readonly AuctionContext _context;

        public ApiTransactionsController(AuctionContext context)
        {
            _context = context;
        }

        // GET: api/Transactions


        [HttpGet]
        public ActionResult<IEnumerable<TransactionViewModel>> GetTransaction()
        {
            var context = _context.Transaction
                .Include(t => t.Customer)
                .Include(t => t.Owner)
                .Include(t => t.Product).AsEnumerable();

            IOrderedEnumerable<TransactionViewModel> transactions = context.Select(t => new TransactionViewModel(
                t.InsertDateTime,
                t.Product.ProductName,
                t.Customer.FullName,
                t.Owner.FullName,
                t.TransactionAmount)).OrderBy(t => t.TransactionDate);


            return transactions.ToList();
        }
    }
}