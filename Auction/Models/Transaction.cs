using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;

namespace Auction.Models
{
    public class Transaction
    {
        public int ID { get; set; }

        [Display(Name = "Transaction date")]
        [DataType(DataType.DateTime)]
        public DateTime InsertDateTime { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "TransactionAmount")]
        public decimal TransactionAmount { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [Display(Name = "Product")]
        public Product Product { get; set; }

        public string OwnerAuctionUserId { get; set; }

        [ForeignKey("OwnerAuctionUserId")]
        [InverseProperty("OwnerTransactions")]
        [Display(Name = "Owner")]
        public AuctionUser Owner { get; set; }

        public string CustomerAuctionUserId { get; set; }

        [ForeignKey("CustomerAuctionUserId")]
        [InverseProperty("CustomerTransactions")]
        [Display(Name = "Customer")]
        public AuctionUser Customer { get; set; }
    }
}