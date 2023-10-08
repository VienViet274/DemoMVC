using DemoMVC.Data;
using DemoMVC.DataAccess.Repository;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DemoMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles ="Admin")]
	public class CategoryController : Controller
	{
        private IUnitOfWork _unitOfWork;
		
        public CategoryController(IUnitOfWork unitOfWork)
        {
			_unitOfWork= unitOfWork;
        }
        public IActionResult MainPage()
        {
            List<Category> ds = _unitOfWork.CategoryRepository.GetAll(null,null).ToList();
            return View(ds);
        }
        [HttpPost]
        public IActionResult MainPage(string TimKiem)
        {
            IEnumerable<Category> categories= new List<Category>();
            if (string.IsNullOrEmpty(TimKiem))
            {
                 categories = _unitOfWork.CategoryRepository.GetAll(null, null);
            }
            else
            {
                categories = _unitOfWork.CategoryRepository.GetAll(x=>x.Name.Contains(TimKiem)||x.NamSinh.ToString().Contains(TimKiem), null);
            }
            return View(categories);
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        { 
            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Add(obj);
                _unitOfWork.CategoryRepository.Save();
                TempData["success"] = "category created successfully";
                return RedirectToAction("MainPage");
            }
            else
            {
                
                TempData["Error"] = "Your input does not meet requirement";
                return View();
            }

        }
        public IActionResult Edit(int? idd)
        {

            if (idd == null || idd == 0)
            {
                return NotFound();
            }
            Category categoryfromDB = _unitOfWork.CategoryRepository.Get(u=>u.ID==idd,null);
            if (categoryfromDB == null)
            {
                return NotFound();
            }
            else
            {
                return View(categoryfromDB);
            }


        }
        [HttpPost]
        public IActionResult Edit(Category ct)
        {
            if (ModelState.IsValid)
            {
				_unitOfWork.CategoryRepository.Update(ct);
				_unitOfWork.CategoryRepository.Save();
				return RedirectToAction("MainPage");
			}
            else
            {
                return View();
            }
            


        }
        public IActionResult Delete(int? idd)
        {
            if (idd == null || idd == 0)
            {
                return NotFound();
            }
            Category ct = _unitOfWork.CategoryRepository.Get(u=>u.ID==idd,null);
            if (ct == null)
            {
                return NotFound();
            }
            else
            {
                return View(ct);
            }

        }
        [HttpPost]
        public IActionResult Delete(Category ct)
        {
            	_unitOfWork.CategoryRepository.Remove(ct);
				_unitOfWork.CategoryRepository.Save();
            	return RedirectToAction("MainPage");
		}
    }
}
