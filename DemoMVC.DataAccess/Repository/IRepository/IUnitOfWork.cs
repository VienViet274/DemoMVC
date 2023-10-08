using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		ICategoryRepository CategoryRepository { get; }
		IProductRepository ProductRepository { get; }
		ICompanyRepository CompanyRepository { get; }
		IShoppingCartRepository ShoppingCartRepository { get; }
		IApplicationUserRepository ApplicationUserRepository { get; }
		IOrderHeaderRepository OrderHeaderRepository { get; }
		IOrderDetailsRepository OrderDetailsRepository { get; }
		void Save();
	}
}
