using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;
using Newtonsoft.Json;

namespace Auction.Models
{
    public class ProductLot
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Lot Name")]
        public string LotName { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Start Price")]
        public decimal StartPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Current Price")]
        public decimal CurrentPrice { get; set; }

        [Display(Name = "Start of trading")]
        [DataType(DataType.DateTime)]
        public DateTime StartTrading { get; set; }

        [Display(Name = "End of trading")]
        [DataType(DataType.DateTime)]
        public DateTime EndTrading { get; set; }

        [Display(Name = "Update Time")]
        [DataType(DataType.DateTime)]
        public DateTime UpdateDateTime { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [Display(Name = "Product")]
        public Product Product { get; set; }

        public string OwnerAuctionUserId { get; set; }

        [ForeignKey("OwnerAuctionUserId")]
        [InverseProperty("OwnerProductLots")]
        [Display(Name = "Owner")]
        public AuctionUser Owner { get; set; }

        public string CustomerAuctionUserId { get; set; }

        [ForeignKey("CustomerAuctionUserId")]
        [InverseProperty("CustomerProductLots")]
        [Display(Name = "Customer")]
        public AuctionUser Customer { get; set; }
    }
}