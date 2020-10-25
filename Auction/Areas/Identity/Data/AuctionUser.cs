using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Auction.Models;
using Microsoft.AspNetCore.Identity;

namespace Auction.Areas.Identity.Data
{
    public class AuctionUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return Name + " " + Surname; }
        }

        [Column(TypeName = "decimal(18, 2)")] public decimal Wallet { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<ProductLot> OwnerProductLots { get; set; }
        public ICollection<ProductLot> CustomerProductLots { get; set; }
        public ICollection<Transaction> CustomerTransactions { get; set; }
        public ICollection<Transaction> OwnerTransactions { get; set; }
    }
}