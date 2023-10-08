using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class OrderDetails
    {
        public int ID { get; set; }
        [Required]
        public int OrderHeaderID { get; set; }
        public OrderHeader Orderheader { get; set; }
        [Required]
        public int ProductID { get; set; }
        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
