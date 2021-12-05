using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auction.Data;
using Auction.Models;
using Auction.Signals;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auction.HostedServices
{
	public class UpdateAuctionHostedService : IHostedService, IDisposable
	{
		private readonly ILogger<UpdateAuctionHostedService> _logger;
		private Timer _timer;
		private readonly AuctionContext _context;
		private readonly IHubContext<NotificationHub> _hubContext;

		public UpdateAuctionHostedService(ILogger<UpdateAuctionHostedService> logger, IServiceScopeFactory factory)
		{
			_logger = logger;
			_context = factory.CreateScope().ServiceProvider.GetRequiredService<AuctionContext>();
			_hubContext = factory.CreateScope().ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
		}

		public void Dispose()
		{
			_timer.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.Log(LogLevel.Information, "UpdateAuctionHostedService is running.");

			_timer = new Timer(UpdateAuction, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		private void UpdateAuction(object state)
		{

			var currentDate = DateTime.Now;
			var auctionTransactions = _context.ProductLot
				.Include(p => p.Customer)
				.Include(p => p.Owner)
				.Include(p => p.Product)
				.Where(p => p.EndTrading < currentDate).ToList().GroupBy(p => p.LotName);



			if (!auctionTransactions.Any())
			{
				return;
			}

			foreach (var t in auctionTransactions)
			{
				
				var currentLotStage = t.FirstOrDefault(x => x.IsActive);
				if (currentLotStage == null)
				{
					continue;
				}

				if (currentLotStage.OwnerAuctionUserId == currentLotStage.CustomerAuctionUserId)
				{
					_context.ProductLot.RemoveRange(t);
					_logger.Log(LogLevel.Information, "the Lot: {0} has expired", currentLotStage.LotName);
				}
				else
				{
					var prevStage = t.Where(x => x.IsActive == false).OrderByDescending(x => x.UpdateDateTime).ToList();
					for (int i = 0; i < prevStage.Count() - 1; i++)
					{
						prevStage[i].Customer.Wallet += prevStage[i].CurrentPrice;
					}

					var transaction = new Transaction()
					{
						ProductId = currentLotStage.ProductId,
						OwnerAuctionUserId = currentLotStage.OwnerAuctionUserId,
						CustomerAuctionUserId = currentLotStage.CustomerAuctionUserId,
						TransactionAmount = currentLotStage.CurrentPrice,
						InsertDateTime = DateTime.Now
					};
					currentLotStage.IsActive = false;
					currentLotStage.Owner.Wallet += currentLotStage.CurrentPrice;
					currentLotStage.Product.AuctionUser = currentLotStage.Customer;
					_context.Transaction.Add(transaction);
					_context.ProductLot.RemoveRange(t);
					_logger.Log(LogLevel.Information, "the Lot: {0} was sold to {}", currentLotStage.LotName, currentLotStage.Customer.FullName);
				}

				
			}

			_context.SaveChanges();
			_hubContext.Clients.All.SendAsync("UpdateAuction", "Lots are updated. Please refresh this page.");

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.Log(LogLevel.Information, "UpdateAuctionHostedService is stopping.");
			return Task.CompletedTask;
		}
	}
}
