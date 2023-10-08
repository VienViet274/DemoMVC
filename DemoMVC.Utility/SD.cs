using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Utility
{
    public static class SD
    {
        public const string Role_User_Customer = "Customer";
        public const string Role_User_Company = "Company";
        public const string Role_User_Admin = "Admin";
        public const string Role_User_Employee = "Employee";

		public const string Status_Pending = "Pending";
		public const string Status_Approved = "Approved";
		public const string Status_Processing = "Processing";
		public const string Status_Shipped = "Shipped";
		public const string Status_Cancelled = "Cancelled";
		public const string Status_Refunded = "Refunded";

		public const string Payment_Status_Pending = "Pending";
		public const string Payment_Status_Approved = "Approved";
		public const string Payment_Status_DelayedPayment = "ApprovedForDelayedPayment";
		public const string Payment_Status_Rejected = "Rejected";

        public const string SessionCart = "SessionShoppingCart";
		public static int GetPrice(ShoppingCart spc)
        {
            int Price = 0;

            if (spc.Count < 50 && spc.Count > 0)
            {
                Price = spc.Product.Price;
            }
            if (spc.Count >= 50 && spc.Count < 100)
            {
                Price = spc.Product.Price50;
            }
            if (spc.Count >= 100)
            {
                Price = spc.Product.Price100;
            }

            return Price;
        }
        
    }
    
}
