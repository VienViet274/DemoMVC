using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DemoMVC.Models
{
	public class ProductVSCategory
	{
		public Product products { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> categories { get; set; }

		
	}
}
