using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Auction.Areas.Identity.Data;

namespace Auction.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Product description")]
        public string ProductDesc { get; set; }

        public string AuctionUserId { get; set; }

        [Display(Name = "Owner")]
        public AuctionUser AuctionUser { get; set; }
        public ProductLot ProductLot { get; set; }
    }
}