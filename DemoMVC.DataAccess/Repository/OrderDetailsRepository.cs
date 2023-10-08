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
    public class OrderDetailsRepository:Repository<OrderDetails>,IOrderDetailsRepository
    {
        private readonly DataContext _context;
        public OrderDetailsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(OrderDetails orderDetails)
        {
           _context.Update(orderDetails);
        }
    }
}
