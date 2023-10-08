using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository:IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void Save();
        void UpdateOrderStatus(OrderHeader ordh, string OrderStatus,string PaymentStatus=null);
        void UpdateStripePaymentID(OrderHeader ordh, string PaymentID, string SessionID);
    }
}
