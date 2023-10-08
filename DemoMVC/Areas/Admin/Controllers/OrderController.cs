using DemoMVC.DataAccess.Repository;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using DemoMVC.Models.ViewModels;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace DemoMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize]
    
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }
        
        public IActionResult Index(string status)
        {
            IEnumerable<OrderHeader> list = new List<OrderHeader>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (string.IsNullOrWhiteSpace(status))
            {
                if (User.IsInRole(SD.Role_User_Admin)||User.IsInRole(SD.Role_User_Employee))
                {
                    list = _unitofwork.OrderHeaderRepository.GetAll(null, "User").ToList();
                    return View(list);
                }
                else
                {
                    
                    list = _unitofwork.OrderHeaderRepository.GetAll(x => x.ApplicationUserID == user,"User");
                    return View(list);
                }
                
            }
            else
            {
                if (User.IsInRole(SD.Role_User_Admin) || User.IsInRole(SD.Role_User_Employee))
                {
                    switch (status)
                    {
                        case "inprocess":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Processing, "User").ToList();
                            break;
                        case "pending":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Pending, "User").ToList();
                            break;
                        case "completed":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Shipped, "User").ToList();
                            break;
                        case "approved":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Approved, "User").ToList();
                            break;
                        case "all":
                            list = _unitofwork.OrderHeaderRepository.GetAll(null, "User").ToList();
                            break;
                    }
                }
                else
                {
                    switch (status)
                    {
                        case "inprocess":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Processing&&x.ApplicationUserID==user, "User").ToList();
                            break;
                        case "pending":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Pending && x.ApplicationUserID == user, "User").ToList();
                            break;
                        case "completed":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Shipped && x.ApplicationUserID == user, "User").ToList();
                            break;
                        case "approved":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Status_Approved && x.ApplicationUserID == user, "User").ToList();
                            break;
                        case "all":
                            list = _unitofwork.OrderHeaderRepository.GetAll(x=> x.ApplicationUserID == user, "User").ToList();
                            break;
                    }
                }
                    
                return View(list);
            }
            
            
            
            
           
        }
        public IActionResult OrderDetail(int? idd)
        {
            if (idd == null || idd == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                OrderVM orderViewModel = new OrderVM()
                {
                    OrderHeader = _unitofwork.OrderHeaderRepository.Get(x => x.ID == idd, "User"),
                OrderDetails= _unitofwork.OrderDetailsRepository.GetAll(x=>x.OrderHeaderID==idd,"Product")
                };
                //orderViewModel.OrderHeader.User = _unitofwork.ApplicationUserRepository.Get(x => x.Id == orderViewModel.OrderHeader.ApplicationUserID, null);
                
                return View(orderViewModel);
            }
        }
        [HttpPost]
        public IActionResult UpdateOrderHeader(OrderVM OrderVM)
        {
            OrderHeader orderHeader = new OrderHeader();
            orderHeader = _unitofwork.OrderHeaderRepository.Get(x => x.ID == OrderVM.OrderHeader.ID, null,true);
            orderHeader.Name= OrderVM.OrderHeader.Name;
            orderHeader.PhoneNumber= OrderVM.OrderHeader.PhoneNumber;
            orderHeader.City= OrderVM.OrderHeader.City;
            orderHeader.State= OrderVM.OrderHeader.State;
            orderHeader.StreetAddress= OrderVM.OrderHeader.StreetAddress;
            orderHeader.PostalCode= OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeader.Carrier= OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitofwork.Save();
            TempData["success"] = "Updated order Header";
            //return RedirectToAction(nameof(OrderDetail), new {idd=OrderVM.OrderHeader.ID});
            return RedirectToAction(nameof(Index), new {idd=OrderVM.OrderHeader.ID});
            
            
        }
        [HttpPost]
        public IActionResult PayNow(OrderVM OrderVM)
        {
            var claimIdentiti = (ClaimsIdentity)User.Identity;
            var user = claimIdentiti.FindFirst(ClaimTypes.NameIdentifier).Value;
            OrderHeader orderHeader = new OrderHeader();
            orderHeader = _unitofwork.OrderHeaderRepository.Get(x => x.ID == OrderVM.OrderHeader.ID, null);
            if (user != orderHeader.ApplicationUserID)
            {
                return NotFound();
            }
            IEnumerable<OrderDetails> orderDetails = new List<OrderDetails>();
            orderDetails = _unitofwork.OrderDetailsRepository.GetAll(x => x.OrderHeaderID == OrderVM.OrderHeader.ID, "Product");
            var options = new SessionCreateOptions
            {
                SuccessUrl = "https://localhost:44326/"+ $"Admin/Order/PaymentDoneCompany?idd={OrderVM.OrderHeader.ID }",
                CancelUrl= "https://localhost:44326/"+$"Admin/Order/Index",
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>()
            };
            foreach (var items in orderDetails)
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
            Session session = service.Create(options);
           
            _unitofwork.OrderHeaderRepository.UpdateStripePaymentID(orderHeader, session.PaymentIntentId, session.Id);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_User_Admin + "," +SD.Role_User_Employee)]
        public IActionResult StartProcessing(OrderVM OrderVM)
        {
            OrderHeader orderHeader=new OrderHeader();
            orderHeader = _unitofwork.OrderHeaderRepository.Get(x => x.ID == OrderVM.OrderHeader.ID, "User");
            /*orderHeader.OrderStatus = SD.Status_Processing;
            _unitofwork.OrderHeaderRepository.Update(orderHeader);
            _unitofwork.Save();*/
            _unitofwork.OrderHeaderRepository.UpdateOrderStatus(orderHeader, SD.Status_Processing);
            
            TempData["success"] = "Order Details update successfully";
            return RedirectToAction(nameof(OrderDetail),new {idd=OrderVM.OrderHeader.ID});
        }
        [HttpPost]
        public IActionResult ShipOrder(OrderVM OrderVM)
        {
            OrderHeader orderHeader = new OrderHeader();
            orderHeader = _unitofwork.OrderHeaderRepository.Get(x => x.ID == OrderVM.OrderHeader.ID, "User");
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier)&& !string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
                orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
                orderHeader.ShippingDate = DateTime.Now;
                orderHeader.OrderStatus = SD.Status_Shipped;
                
                if (orderHeader.PaymentStatus == SD.Payment_Status_DelayedPayment)
                {
                    orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
                }
                _unitofwork.OrderHeaderRepository.Update(orderHeader);
                _unitofwork.Save();
                TempData["success"] = "Carrier and tracking number  updated successfully";
                return RedirectToAction(nameof(OrderDetail), new {idd=OrderVM.OrderHeader.ID});
               
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            OrderHeader orderHeader = new OrderHeader();
            orderHeader = _unitofwork.OrderHeaderRepository.Get(x => x.ID == orderVM.OrderHeader.ID, "User");
            if (orderVM.OrderHeader.PaymentStatus == SD.Payment_Status_Approved )
            {
                
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentID
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitofwork.OrderHeaderRepository.UpdateOrderStatus(orderHeader, SD.Status_Cancelled, SD.Status_Refunded);
            }
            else
            {
                _unitofwork.OrderHeaderRepository.UpdateOrderStatus(orderHeader, SD.Status_Cancelled, SD.Status_Cancelled);
            }
            TempData["success"] = "Carrier and tracking number  updated successfully";
            return RedirectToAction(nameof(OrderDetail), new { idd = orderVM.OrderHeader.ID });
        }
        public IActionResult PaymentDoneCompany(int? idd)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var user = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (idd == null || idd == 0)
            {
                return NotFound();
            }
            else
            {
                OrderHeader orderHeader = new OrderHeader();
                orderHeader = _unitofwork.OrderHeaderRepository.Get(u => u.ID == idd, null);
                

                var service = new SessionService();
                var sessionID=service.Get(orderHeader.SessionID);
                if (sessionID.PaymentStatus.ToLower() == "paid")
                {
                    _unitofwork.OrderHeaderRepository.UpdateOrderStatus(orderHeader, SD.Status_Approved, SD.Payment_Status_Approved);
                    _unitofwork.OrderHeaderRepository.UpdateStripePaymentID(orderHeader, sessionID.PaymentIntentId, sessionID.Id);
                    IEnumerable<ShoppingCart> spc = new List<ShoppingCart>();
                    spc = _unitofwork.ShoppingCartRepository.GetAll(x => x.UserID == user, "Product");
                    _unitofwork.ShoppingCartRepository.RemoveRange(spc);
                    _unitofwork.Save();

                }
                
               
                return View();
            }
        }
    }
}
