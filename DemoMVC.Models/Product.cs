using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
	public class Product
	{
		[Key]
		public int ID { get; set; }
		[Required]
		public string Title { get; set; }
		public string Description { get; set; }
		[Required]
		public string ISBN { get; set; }
		[Required]
		public string Author { get; set; }
		[Required]
		public int ListPrice { get; set; }
		[Required]
		public int Price { get; set; }
		[Required]
		public int Price50 { get; set; }
		[Required]
		
		public int Price100 { get; set; }
		[ValidateNever]
		
		public string? ImageURl { get; set; }
		[ValidateNever]
		public int CategoryID { get; set; }
		[ForeignKey(nameof(CategoryID))]
		[ValidateNever]
		public Category Category { get; set; }

	}
}
