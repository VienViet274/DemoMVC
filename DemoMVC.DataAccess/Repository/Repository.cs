using DemoMVC.Data;
using DemoMVC.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DemoMVC.DataAccess.Repository
{
	public class Repository<T>:IRepository<T> where T : class
	{
		private DataContext _db;
		internal DbSet<T> _dbSet;
		public Repository(DataContext db)
		{
			_db = db;
			_dbSet = _db.Set<T>();
			_db.Products.Include(u => u.Category);
			_db.ShoppingCarts.Include(u => u.Product).Include(u=>u.User);
			_db.OrderHeaders.Include(u => u.User);
			_db.ApplicationUser.Include(u => u.CompanyKey);
			
			
		}

		public void Add(T entity)
		{
			_dbSet.Add(entity);
		}
		public T Get(Expression<Func<T, bool>> filter, string stringProperties,bool tracked=false)
		{
			IQueryable<T> querry;

            if (tracked == true)
			{
                querry= _dbSet;
                querry = querry.Where(filter);
                if (stringProperties != null)
                {
					
                    foreach (var includeProp in stringProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        querry = querry.Include(includeProp);
                    }

                }
            }
			else
			{
                querry = _dbSet.AsNoTracking();
                querry = querry.Where(filter);
                if (stringProperties != null)
                {
                    foreach (var includeProp in stringProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        querry = querry.Include(includeProp);
                    }
                }
            }
			
			return querry.FirstOrDefault();
		}
		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string stringProperties)
		{
            IQueryable<T> querry = _dbSet;
            if (filter != null)
			{
                
                querry = querry.Where(filter);
				               
            }
            if (stringProperties != null)
            {
                
                foreach (var includeProp in stringProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
                    querry = querry.Include(includeProp);
                }
            }

            return querry.ToList();

		}
		public void Remove(T entity)
		{
			_dbSet.Remove(entity);
		}
		public void RemoveRange(IEnumerable<T> entity)
		{
			_dbSet.RemoveRange(entity);
		}
	}
}
