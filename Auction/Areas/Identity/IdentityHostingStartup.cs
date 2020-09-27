using System;
using Auction.Areas.Identity.Data;
using Auction.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Auction.Areas.Identity.IdentityHostingStartup))]
namespace Auction.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AuctionContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AuctionContextConnection")));

                services.AddDefaultIdentity<AuctionUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<AuctionContext>();
            });
        }
    }
}