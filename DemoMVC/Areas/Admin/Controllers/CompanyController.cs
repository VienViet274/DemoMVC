using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_User_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public IActionResult MainPage()
        {
            IEnumerable<Company>ds=new List<Company>();
            ds=_unitOfWork.CompanyRepository.GetAll(null,null).ToList();
            return View(ds);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Company obj)
        {
            _unitOfWork.CompanyRepository.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Company created successfully";
            return RedirectToAction("MainPage");
        }
        public IActionResult Edit(int? IDD)
        {
            if(IDD==0 || IDD == null)
            {
                return NotFound();
            }
            else
            {                
                    Company obj = _unitOfWork.CompanyRepository.Get(x => x.ID == IDD, null);
                if (obj == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(obj);
                }
                    
            }
        }
        [HttpPost]
        public IActionResult Edit(Company obj)
        {
            if (ModelState.IsValid)
            {
				_unitOfWork.CompanyRepository.Update(obj);
				_unitOfWork.Save();
				return RedirectToAction("MainPage");
			}
            else
            {
                return View(obj);
            }
           
        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Company> ds = new List<Company>();
            ds = _unitOfWork.CompanyRepository.GetAll(null,null).ToList();
            return Json(new{data = ds} );
        }
        
        public IActionResult Delete(int? idd)
        {
            if (idd == 0 || idd == null)
            {
                return Json(new {success="false",message="Delete Can not be aplied"});
            }
            else
            {
                Company obj=_unitOfWork.CompanyRepository.Get(x=>x.ID==idd, null);
                if (obj.ID != 0)
                {
                    _unitOfWork.CompanyRepository.Remove(obj);
                    _unitOfWork.Save();
                    return Json(new { success = "true", messsage = "Delete successfully" });
                }
                else
                {
                    return Json(new { success = "false", message = "Delete Can not be aplied" });
                }
            }
        }
        #endregion
    }
}
