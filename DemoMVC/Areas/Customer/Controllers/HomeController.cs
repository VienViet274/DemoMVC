using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace DemoMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MainPage()
        {
            IEnumerable<Product> dsProduct = _unitOfWork.ProductRepository.GetAll(null, "Category").ToList();
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == claim.Value, null).Count());
            }
            
            



            return View(dsProduct);
        }
        public IActionResult ProductDetail(int? IDD)
        {
            ShoppingCart spc = new ShoppingCart();
            if (IDD == 0 || IDD == null)
            {
                return NotFound();
            }
            else
            {
                spc = new() { Product = _unitOfWork.ProductRepository.Get(x => x.ID == IDD, "Category"), Count = 1 };
            }

            if (spc.Product.ID == 0 || spc.Product == null)
            {
                return NotFound();
            }
            else
            {
                spc.ProductID = spc.Product.ID;
                return View(spc);
            }
        }
        [HttpPost]
        [Authorize]
    public IActionResult ProductDetail(ShoppingCart spc)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var user = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            spc.UserID= user;
            spc.Product=_unitOfWork.ProductRepository.Get(x=>x.ID==spc.ProductID, "Category",true);
            ShoppingCart obj = new ShoppingCart();
            obj=_unitOfWork.ShoppingCartRepository.Get(x=>x.UserID== spc.UserID&&x.ProductID==spc.ProductID,null);
            if (spc.Count == 0)
            {
                return RedirectToAction(nameof(MainPage));
            }
            if (obj != null)
            {
                obj.Count += spc.Count;
                obj.Price = SD.GetPrice(spc);
                _unitOfWork.ShoppingCartRepository.Update(obj);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == spc.UserID, null).Count());
                TempData["success"] = "Updated product in your shopping cart";
            }
            else
            {
                spc.Price= SD.GetPrice(spc);
                _unitOfWork.ShoppingCartRepository.Add(spc);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepository.GetAll(x => x.UserID == spc.UserID, null).Count());
               
                TempData["success"] = "added product to your shopping cart";
            }
            return RedirectToAction("MainPage");
            /*List<ShoppingCart> ds = _unitOfWork.ShoppingCartRepository.GetAll(null).ToList();
            bool flag = false;
            ShoppingCart bienTam = new ShoppingCart();
            for(int i=0;i<ds.Count; i++)
            {
                if (ds[i].UserID == spc.UserID && ds[i].ProductID == spc.ProductID)
                {
                    flag=true;
                    bienTam = ds[i];
                }
            }
            if(flag==true)
            {
                bienTam.Count += spc.Count;
               // _unitOfWork.ShoppingCartRepository.Update(bienTam);
                _unitOfWork.Save();
                TempData["success"] = "Updated product to your shopping cart";
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.Add(spc);
                _unitOfWork.Save();
                TempData["success"] = "added product to your shopping cart";
            }
                
                return RedirectToAction("MainPage");*/




        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}