using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;
using Auction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auction.Data
{
    public class AuctionContext : IdentityDbContext<AuctionUser>
    {
        public DbSet<Auction.Models.Product> Product { get; set; }
        public DbSet<Auction.Models.ProductLot> ProductLot { get; set; }
        public DbSet<Auction.Models.Transaction> Transaction { get; set; }
        public AuctionContext(DbContextOptions<AuctionContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
