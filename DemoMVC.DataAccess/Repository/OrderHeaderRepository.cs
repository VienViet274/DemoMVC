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
    public class OrderHeaderRepository:Repository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly DataContext _context;
        public OrderHeaderRepository(DataContext context):base(context) 
        {
            _context = context;
        }

        public void Save()
        {
           _context.SaveChanges();
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.Update(orderHeader);
        }

        public void UpdateOrderStatus(OrderHeader ordh, string OrderStatus, string PaymentStatus = null)
        {
            ordh.OrderStatus = OrderStatus;
            if(!string.IsNullOrEmpty(PaymentStatus) )
            {
                ordh.PaymentStatus = PaymentStatus;
            }
            _context.Update(ordh);
            _context.SaveChanges();
        }

        public void UpdateStripePaymentID(OrderHeader ordh, string PaymentID, string SessionID)
        {
            ordh.PaymentIntentID= PaymentID;
            ordh.SessionID= SessionID;
            ordh.PaymentDate = DateTime.Now;
            _context.Update(ordh);
            _context.SaveChanges();
        }
    }
}
