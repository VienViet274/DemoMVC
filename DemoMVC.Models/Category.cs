using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
	public class Category
	{
		[Key]
		public int ID { get; set; }
		[Required]
		[MaxLength(30)]
		
		public string Name { get; set; }
		[Range(0,3000)]
		public int NamSinh { get; set; }
	}
}
