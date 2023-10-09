using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles =SD.Role_User_Admin)]
    public class UserController : Controller
    {
        private DataContext _context;
        public UserController(DataContext context) {
            _context= context;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationUser> ds=_context.ApplicationUser.Include("CompanyKey").ToList();
            
           /* IEnumerable<ApplicationUser>ds= _context.ApplicationUser.ToList();
            _context.Database.CloseConnection();
            foreach(var a in ds)
            {
                a.CompanyKey = _context.Companies.Where(x => x.ID == a.CompanyID).FirstOrDefault();
            }*/
            
            
            return View(ds);
        }
    }
}
