using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? City { get; set; }
        public string? StreetAddress { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        [ValidateNever]
        public int? CompanyID { get; set; }
		[ForeignKey(nameof(CompanyID))]
		[ValidateNever]
		public Company? CompanyKey { get; set; }
        [NotMapped]
        public string UserRole { get; set; }
        
    }
}
