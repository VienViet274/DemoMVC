using DemoMVC.DataAccess.Repository;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using System.Security.Cryptography;

namespace DemoMVC.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        /*[BindProperty]
        public CartViewModel cvm { get; set; }*/
        public CartController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork= UnitOfWork;
        }
        public IActionResult MainPage()
        {
            var claimIdentity=(ClaimsIdentity)User.Identity;
            var user = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartViewModel ds= new CartViewModel();
            ds = new()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == user, "Product,User"),
                
           
            };
            CartViewModel dsfinal = new CartViewModel();
            dsfinal = new()
            {
                ShoppingCartList = ds.ShoppingCartList,
                OrderHeaders = new()
                {
                    OrderTotal=GetOrderToTal(ds)
                }
            };
           // IEnumerable<ShoppingCart> ds = _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == user,"Product");

            /*List<ShoppingCart> ds=new List<ShoppingCart>();
           List<ShoppingCart> data=new List<ShoppingCart>();
            data = _UnitOfWork.ShoppingCartRepository.GetAll("Product").ToList();
            for(int i=0;i<data.Count; i++)
            {
                ShoppingCart spc = new ShoppingCart();
                if (data[i].UserID == user)
                {
                    spc = data[i];
                    ds.Add(spc);
                }
                
            }*/
            
            return View(dsfinal);
        }
        public IActionResult Plus(int? idd)
        {
            if (idd == null || idd == 0)
            {
                return NotFound();
            }
            else
            {
                ShoppingCart spm= new ShoppingCart();
                 spm= _UnitOfWork.ShoppingCartRepository.Get(x => x.ID == idd, "Product,User");
                spm.Count += 1;
                spm.Price = SD.GetPrice(spm);
                _UnitOfWork.ShoppingCartRepository.Update(spm);
                _UnitOfWork.Save();
                return RedirectToAction("MainPage");
            }
        }
        public IActionResult Minus(int? idd)
        {
            if (idd == null || idd == 0)
            {
                return NotFound();
            }
            else
            {
                ShoppingCart spm = new ShoppingCart();
                spm = _UnitOfWork.ShoppingCartRepository.Get(x => x.ID == idd, "Product");
                spm.Count -= 1;
                spm.Price = SD.GetPrice(spm);
                _UnitOfWork.ShoppingCartRepository.Update(spm);
                _UnitOfWork.Save();
                return RedirectToAction("MainPage");
            }
        }
        public IActionResult Summary()
        {
            var IdentityClaims = (ClaimsIdentity)User.Identity;
            var user = IdentityClaims.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartViewModel cvm= new CartViewModel();
            cvm = new()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == user, "Product,User")

            };
            CartViewModel ds = new CartViewModel();
            ds = new()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == user, "Product,User"),
                OrderHeaders = new()
                {

                    OrderTotal = GetOrderToTal(cvm),
                    ApplicationUserID=user,
                    Name=_UnitOfWork.ApplicationUserRepository.Get(x=>x.Id==user,null).Name,
                    PhoneNumber=_UnitOfWork.ApplicationUserRepository.Get(x=>x.Id==user,null).PhoneNumber,
                    StreetAddress=_UnitOfWork.ApplicationUserRepository.Get(x=>x.Id==user,null).StreetAddress,
                    State=_UnitOfWork.ApplicationUserRepository.Get(x=>x.Id==user,null).State,
                    PostalCode=_UnitOfWork.ApplicationUserRepository.Get(x=>x.Id==user,null).PostalCode,
                    City=_UnitOfWork.ApplicationUserRepository.Get(x=>x.Id==user,null).City
                } 
            };
            return View(ds);
            
        }
        [HttpPost]
        [Authorize]
        [ActionName(nameof(Summary))]
        public IActionResult SummaryPOST(CartViewModel CartViewModel)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var user=claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartViewModel.ShoppingCartList = _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == user,"User,Product");
            CartViewModel.OrderHeaders.ApplicationUserID = user;
            CartViewModel.OrderHeaders.User = _UnitOfWork.ApplicationUserRepository.Get(x => x.Id == user, null,true);
            CartViewModel.OrderHeaders.OrderDate = DateTime.Now;
            CartViewModel.OrderHeaders.OrderTotal = GetOrderToTal(CartViewModel);
            CartViewModel.OrderHeaders.OrderDate=DateTime.Now;
            CartViewModel.OrderHeaders.ShippingDate = CartViewModel.OrderHeaders.OrderDate.AddDays(7);
            if (CartViewModel.OrderHeaders.User.CompanyID.HasValue)
            {
                CartViewModel.OrderHeaders.OrderStatus = SD.Status_Approved;
                CartViewModel.OrderHeaders.PaymentStatus = SD.Payment_Status_DelayedPayment;
                _UnitOfWork.OrderHeaderRepository.Add(CartViewModel.OrderHeaders);
                _UnitOfWork.Save();
                foreach (var a in CartViewModel.ShoppingCartList)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.OrderHeaderID = CartViewModel.OrderHeaders.ID;
                    orderDetails.ProductID = a.ProductID;
                    orderDetails.Count = a.Count;
                    orderDetails.Price = a.Price;
                    _UnitOfWork.OrderDetailsRepository.Add(orderDetails);
                    _UnitOfWork.Save();
                }
            }
            else
            {
                CartViewModel.OrderHeaders.OrderStatus = SD.Status_Pending;
				
				_UnitOfWork.OrderHeaderRepository.Add(CartViewModel.OrderHeaders);
				_UnitOfWork.Save();
                var options = new SessionCreateOptions
                {
                    SuccessUrl = "https://localhost:44326/"+$"Customer/Cart/Orderconfirmation?idd={CartViewModel.OrderHeaders.ID}",
                    CancelUrl= "https://example.com/success",
                    Mode = "payment",
                    LineItems = new List<SessionLineItemOptions>()
                 
                };
                foreach (var items in CartViewModel.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(items.Product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = items.Product.Title
                            }
                        },
                        Quantity = items.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session= service.Create(options);
               _UnitOfWork.OrderHeaderRepository.UpdateStripePaymentID(CartViewModel.OrderHeaders, session.PaymentIntentId, session.Id);
                foreach (var a in CartViewModel.ShoppingCartList)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.OrderHeaderID = CartViewModel.OrderHeaders.ID;
                    orderDetails.ProductID = a.ProductID;
                    orderDetails.Count = a.Count;
                    orderDetails.Price = a.Price;
                    _UnitOfWork.OrderDetailsRepository.Add(orderDetails);
                    _UnitOfWork.Save();
                }
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
           
                return RedirectToAction(nameof(OrderConfirmation),new {idd=CartViewModel.OrderHeaders.ID});
            
        }
        public IActionResult OrderConfirmation(int? idd)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            int iddd = 0;
            var userIDLogin = _UnitOfWork.ApplicationUserRepository.Get(x => x.Id == user, null);
            if(idd== null||idd==0)
            {
                return NotFound();
            }
            else
            {
                if(userIDLogin.CompanyID.HasValue)
                {
                    OrderHeader orderHeaderCompany = new OrderHeader();
                    orderHeaderCompany = _UnitOfWork.OrderHeaderRepository.Get(x => x.ID == idd, null);
                    return View(orderHeaderCompany.ID);
                }
                else
                {
                    iddd = idd.Value;
                    OrderHeader orderHeader = new OrderHeader();
                    orderHeader = _UnitOfWork.OrderHeaderRepository.Get(x => x.ID == iddd, null);
                    var service = new SessionService();
                    var session = service.Get(orderHeader.SessionID);
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _UnitOfWork.OrderHeaderRepository.UpdateOrderStatus(orderHeader, SD.Status_Approved, SD.Payment_Status_Approved);
                        _UnitOfWork.OrderHeaderRepository.UpdateStripePaymentID(orderHeader, session.PaymentIntentId, session.Id);
                    }
                    IEnumerable<ShoppingCart> list = _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == orderHeader.ApplicationUserID, null);
                    foreach (var a in list)
                    {
                        _UnitOfWork.ShoppingCartRepository.Remove(a);
                        _UnitOfWork.Save();
                    }
                    HttpContext.Session.Clear();

                    return View(iddd);
                }
                 
            }
            
        }
		#region Sum Method
		public double GetOrderToTal(CartViewModel cartViewModel)
        {
            double GiaTien = 0;
            double Tong = 0;
            foreach(var a in cartViewModel.ShoppingCartList)
            {
                if (a.Count >= 0 && a.Count < 50)
                {
                    GiaTien= a.Count*a.Product.Price;
                }
                else if (a.Count >= 50 && a.Count < 100)
                {
                    GiaTien = a.Count * a.Product.Price50;
                }
                else if (a.Count >= 100)
                {
                    GiaTien = a.Count * a.Product.Price100;
                }
                Tong += GiaTien;
            }
            return Tong;
        }
        #endregion
        #region API CALLS
        public IActionResult Delete(int? idd)
        {
            if (idd == null)
            {
                return Json(new { success = "false", message = "No product to delete" });
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                ShoppingCart spc = _UnitOfWork.ShoppingCartRepository.Get(x => x.UserID == user && x.ID == idd, null);
                if (spc != null)
                {
                    _UnitOfWork.ShoppingCartRepository.Remove(spc);
                    _UnitOfWork.Save();
                    HttpContext.Session.SetInt32(SD.SessionCart, _UnitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == spc.UserID, null).Count());
                    return Json(new { success = "true", message = "Delete successfully" });
                }
                else
                {
                    return Json(new { success = "false", message = "No product to delete" });
                    
                }
            }
        }
        #endregion


    }
}
        
    

