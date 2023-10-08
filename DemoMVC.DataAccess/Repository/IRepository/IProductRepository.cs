using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository.IRepository
{
	public interface IProductRepository:IRepository<Product>
	{
		void Update(Product product);
		void Save();
	}
}
