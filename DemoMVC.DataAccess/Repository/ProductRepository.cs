using DemoMVC.Data;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository
{
	public class ProductRepository:Repository<Product>,IProductRepository
	{
		private DataContext _db;
		public ProductRepository(DataContext db) : base(db)
		{
			_db = db;
		}
		

		public void Update(Product product)
		{
			_db.Update(product);
		}
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
