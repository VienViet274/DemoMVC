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
	public class CategoryRepository:Repository<Category>,ICategoryRepository
	{
		private DataContext _db;
		public CategoryRepository(DataContext db):base(db)
		{
			_db = db;
		}

		public void Update(Category category)
		{
			_db.Update(category);
		}
		public void Save() 
		{
			_db.SaveChanges();
		}
	}
}
