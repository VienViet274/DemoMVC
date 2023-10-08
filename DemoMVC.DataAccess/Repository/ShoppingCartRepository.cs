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
    public class ShoppingCartRepository: Repository<ShoppingCart>,IShoppingCartRepository
    {
        private readonly DataContext _datacontext;
        public  ShoppingCartRepository(DataContext dataContext):base(dataContext) 
        {
            _datacontext = dataContext;
        }

        public void Save()
        {
            _datacontext.SaveChanges();
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _datacontext.Update(shoppingCart);
        }
    }
}
