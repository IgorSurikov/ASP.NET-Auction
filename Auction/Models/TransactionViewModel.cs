using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Models
{
    public class TransactionViewModel
    {
        public DateTime TransactionDate { get; }
        public string Product { get; }
        public string Customer { get; }
        public string Owner { get; }
        public decimal Amount { get; }

        public TransactionViewModel(DateTime transactionDate, string product, string customer, string owner,
            decimal amount)
        {
            TransactionDate = transactionDate;
            Product = product;
            Customer = customer;
            Owner = owner;
            Amount = amount;
        }
    }
}