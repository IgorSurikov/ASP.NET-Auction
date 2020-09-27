using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Auction.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AuctionUser class
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
            get
            {
                return Name + " " + Surname;
            }
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Wallet { get; set; }
    }
}
