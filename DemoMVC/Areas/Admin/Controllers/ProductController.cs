using DemoMVC.Data;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;


namespace DemoMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IUnitOfWork _unitofwork;

		
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitofwork= unitOfWork;
			_webHostEnvironment= webHostEnvironment;
			
		}
		public Product products =new Product();
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult MainPage()
		{
			List<Product> ds = new List<Product>();
			ds = _unitofwork.ProductRepository.GetAll(null,"Category").ToList();
			return View(ds);
		}
		[HttpPost]
		public IActionResult MainPage(string TuKhoa)
		{
			IEnumerable<Product>products=new List<Product>();
			if (string.IsNullOrEmpty(TuKhoa))
			{
				products = _unitofwork.ProductRepository.GetAll(null, "Category").ToList();
			}
			else
			{
                products = _unitofwork.ProductRepository.GetAll(x=>x.Title.Contains(TuKhoa), "Category").ToList();
            }
			return View(products);
		}
		public IActionResult Create()
		{
			

			IEnumerable<SelectListItem> CategoryList= new List<SelectListItem>();
			CategoryList = _unitofwork.CategoryRepository.GetAll(null,null).ToList().Select(u => new SelectListItem { Text = u.Name,Value=u.ID.ToString() });
			
			ViewData["CategoryID"] = CategoryList;
			ProductVSCategory sp = new ProductVSCategory()
			{
				products = new Product(),
				categories = CategoryList
			};
			return View();
		}
		[HttpPost]
		public IActionResult Create(ProductVSCategory sp, IFormFile? linkHinh)
		{
			
			if (ModelState.IsValid)
			{
				string rootPath = _webHostEnvironment.WebRootPath;
				if (linkHinh != null)
				{
					
					string FileImageName = Guid.NewGuid().ToString() + Path.GetExtension(linkHinh.FileName);
					string ProductPath = @"images\products";
                    string finalPath= Path.Combine(rootPath, ProductPath);
                    var fileLuu=new FileStream(Path.Combine(finalPath,FileImageName), FileMode.Create);
					linkHinh.CopyTo(fileLuu);
					fileLuu.Close();
					sp.products.ImageURl = FileImageName;
					
				}
				
				_unitofwork.ProductRepository.Add(sp.products);
				_unitofwork.ProductRepository.Save();
				return RedirectToAction("MainPage", "Product");


			}
			else
			{
				return View();
			}
		}
		public IActionResult Edit(int? IDD)
		{
			if (IDD == 0||IDD==null)
			{
				return NotFound();
			}
			else
			{
				IEnumerable<SelectListItem>CategoryList=new List<SelectListItem>();
				CategoryList = _unitofwork.CategoryRepository.GetAll(null,null).ToList().Select(u=> new SelectListItem {Text=u.Name,Value=u.ID.ToString() } ) ;
				ViewData["CategoryID"] = CategoryList;
				ProductVSCategory sp = new ProductVSCategory();
				sp.products = _unitofwork.ProductRepository.Get(u=>u.ID==IDD,"Category");
				if (sp.products != null)
				{
					return View(sp);
				}
				else
				{
					return NotFound();
				}
			}
		}
		[HttpPost]
		public IActionResult Edit(ProductVSCategory sp,IFormFile? linkHinh)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrEmpty(sp.products.ImageURl))
				{ 
                 sp.products.ImageURl = _unitofwork.ProductRepository.Get(u => u.ID == sp.products.ID, "Category").ImageURl;  
                }
				if (linkHinh != null)
				{
					string rootPath = _webHostEnvironment.WebRootPath;
					string ProductImage = Guid.NewGuid().ToString() + Path.GetExtension(linkHinh.FileName);
					string ImagePath = Path.Combine(rootPath, "Images\\Products");
					var fileLuuHinh = new FileStream(Path.Combine(ImagePath, ProductImage), FileMode.Create);
					linkHinh.CopyTo(fileLuuHinh);
					fileLuuHinh.Close();
					sp.products.ImageURl = ProductImage;
				}
				else
				{
					sp.products.ImageURl = null;

                }
				_unitofwork.ProductRepository.Update(sp.products);
				_unitofwork.ProductRepository.Save();
				return RedirectToAction("MainPage");
			}
			else
			{
				return NotFound();
			}
			
		}    
		        
		/*public IActionResult Delete(int? idd)
		{
			Product obj = new Product();
			if(idd == null || idd == 0)
			{
				return NoContent();
			}
			else
			{
				obj = _unitofwork.ProductRepository.Get(x => x.ID == idd, "Category");
				if (obj.ID == 0)
				{
                    return NoContent();
                }
				else
				{
					
                    return View(obj);
                }
			}
        
        }
        [HttpPost]
        public IActionResult Delete(Product obj)
		{
            _unitofwork.ProductRepository.Remove(obj);
            _unitofwork.Save();
			TempData["success"] = "product Deleted successfully";
			return RedirectToAction("MainPage","Product");
        }*/
        [HttpGet]
		public IActionResult GetAll()
		{
			var table = _unitofwork.ProductRepository.GetAll(null, "Category").ToList();
			return Json(new { data = table });
		}
        
		public IActionResult Delete(int? idd)
		{
			Product obj = new Product();
			if (idd == 0 || idd == null)
			{
				return Json(new { success = "false", message = "error" });
			}
			else
			{
				obj = _unitofwork.ProductRepository.Get(x => x.ID == idd, "Category");
				if (obj.ID == 0)
				{
                    return Json(new { success = "false", message = "error" });
                }
				else
				{
					_unitofwork.ProductRepository.Remove(obj);
					_unitofwork.Save();
                    return Json(new { success = "true", message = "Delete item successfully" });
                }
			}
			
		}
    }
}
