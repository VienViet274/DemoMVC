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
    public class ShoppingCart
    {
        [Key]
        public int ID { get; set; }
        public int ProductID { get; set; }
        [ForeignKey(nameof(ProductID))]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 1000,ErrorMessage ="count between 1-1000")]
        public int Count { get; set; }
        public string UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        [ValidateNever]
        public ApplicationUser User { get; set; }
        public int Price { get; set; }
    }
}
