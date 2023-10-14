using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.DataAccess.Repository;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace DemoMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles =SD.Role_User_Admin)]
    public class UserController : Controller
    {
        [BindProperty]
        public UserAndRoleListVM UserAndRoleListVM { get; set; }
        private DataContext _data;
        private UserManager<IdentityUser> _UserManager;
        public UserController(DataContext context, UserManager<IdentityUser> UserManager) {
            _data= context;
            _UserManager= UserManager;
        }

        public IActionResult Index()
        {
            List<ApplicationUser> ds=_data.ApplicationUser.Include("CompanyKey").ToList();
            var UserRoles=_data.UserRoles.ToList();
            var Roles=_data.Roles.ToList();
            
            foreach(var user in ds)
            {
                foreach(var userrole in UserRoles)
                {
                    if (user.Id == userrole.UserId)
                    {
                        var Encryptrole = userrole.RoleId;
                        foreach(var role in Roles)
                        {
                            if (Encryptrole == role.Id)
                            {
                                user.UserRole = role.Name;
                                break;
                            }
                        }
                    }
                }
                //var Encryptrole = UserRoles.FirstOrDefault(u => u.UserId==user.Id);
                //var role = Roles.FirstOrDefault(u => u.Id == Encryptrole).Name;
                //user.UserRole = role;
                //a.UserRole = Roles.Where(x => x.Id == Encryptrole).FirstOrDefault().Name;
            }
            
           /* IEnumerable<ApplicationUser>ds= _context.ApplicationUser.ToList();
            _context.Database.CloseConnection();
            foreach(var a in ds)
            {
                a.CompanyKey = _context.Companies.Where(x => x.ID == a.CompanyID).FirstOrDefault();
            }*/
            
            
            return View(ds);
        }
        [HttpPost]
        [ActionName(nameof(Index))]
        public IActionResult IndexPOST(string TimKiem)
        {
            if (string.IsNullOrEmpty(TimKiem))
            {
				IEnumerable<ApplicationUser> ds = _data.ApplicationUser.ToList();
				return View(ds);
			}
            else
            {
				 IEnumerable<ApplicationUser>ds =_data.ApplicationUser.Where(x=>x.UserName.Contains(TimKiem)||x.Email.Contains(TimKiem)||x.Name.Contains(TimKiem)).Include(x=>x.CompanyKey).ToList();
				//IEnumerable<ApplicationUser> ds = _data.GetAll(x => x.UserName.Contains(TimKiem) || x.Email.Contains(TimKiem) || x.Name.Contains(TimKiem),"CompanyKey").ToList();
				var UserRoles = _data.UserRoles.ToList();
				var Roles = _data.Roles.ToList();

				foreach (var user in ds)
				{
					foreach (var userrole in UserRoles)
					{
						if (user.Id == userrole.UserId)
						{
							var Encryptrole = userrole.RoleId;
							foreach (var role in Roles)
							{
								if (Encryptrole == role.Id)
								{
									user.UserRole = role.Name;
									break;
								}
							}
						}
					}
					
				}
				return View(ds);
            }
        }
        public IActionResult Unlock(string idd)
        {
            if(string.IsNullOrEmpty(idd))
            {
                return NotFound();
            }
            else
            {
				var user = _data.ApplicationUser.Where(x => x.Id == idd).FirstOrDefault();
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    if (user.LockoutEnd != null)
                    {
                        user.LockoutEnd = null;
                        _data.SaveChanges();
						TempData["success"] = "Unlocked user successfully ";
					}
                    return RedirectToAction(nameof(Index));
                }
			}
            

        }
		public IActionResult Lock(string idd)
        {
			if (string.IsNullOrEmpty(idd))
			{
				return NotFound();
			}
			else
			{
				var user = _data.ApplicationUser.Where(x => x.Id == idd).FirstOrDefault();
				if (user == null)
				{
					return NotFound();
				}
				else
				{
					if (user.LockoutEnd == null)
					{
						user.LockoutEnd = DateTime.Now.AddYears(1000);
						_data.SaveChanges();
                        TempData["success"] = "Locked user successfully ";
					}
					return RedirectToAction(nameof(Index));
				}
			}
		}
        public IActionResult Permission(string idd)
        {
            UserAndRoleListVM userAndRoleListVM = new UserAndRoleListVM();
            userAndRoleListVM.applicationUser = _data.ApplicationUser.Where(x => x.Id == idd).FirstOrDefault();
            userAndRoleListVM.RoleList = _data.Roles.ToList().Select(u=>new SelectListItem { Text=u.Name,Value=u.Name} );
            userAndRoleListVM.CompanyList=_data.Companies.ToList().Select(u=>new SelectListItem { Text=u.Name,Value=u.ID.ToString()});  
            return View(userAndRoleListVM);
        }
        [HttpPost]
		public  IActionResult Permission()
        {
            var StringOldRole = _data.UserRoles.Where(x => x.UserId == UserAndRoleListVM.applicationUser.Id).FirstOrDefault();
            var OldRole=_data.Roles.Where(x=>x.Id==StringOldRole.RoleId).FirstOrDefault().Name;
            var NewRole = _data.Roles.Where(x => x.Name == UserAndRoleListVM.applicationUser.UserRole).FirstOrDefault();
            var RoleID = NewRole.Id;
            _data.Database.CloseConnection();
            
            var userRoleTable = _data.UserRoles.ToList();
            if (NewRole.Name != SD.Role_User_Company)
            {
				foreach (var role in userRoleTable)
				{
					if (role.UserId == UserAndRoleListVM.applicationUser.Id)
					{
						var appUser = _data.ApplicationUser.Where(x => x.Id == UserAndRoleListVM.applicationUser.Id).FirstOrDefault();
						var result1 = _UserManager.RemoveFromRoleAsync(appUser, OldRole).GetAwaiter().GetResult();
						var result2 = _UserManager.AddToRoleAsync(appUser, NewRole.Name).GetAwaiter().GetResult();
						_data.SaveChanges();
						break;
					}
				}
			}
            else
            {
				foreach (var role in userRoleTable)
				{
					if (role.UserId == UserAndRoleListVM.applicationUser.Id)
					{
						
                        var appUser = _data.ApplicationUser.Where(x => x.Id == UserAndRoleListVM.applicationUser.Id).FirstOrDefault();
						var result1 = _UserManager.RemoveFromRoleAsync(appUser, OldRole).GetAwaiter().GetResult();
						var result2 = _UserManager.AddToRoleAsync(appUser, NewRole.Name).GetAwaiter().GetResult();
						appUser.CompanyID = UserAndRoleListVM.applicationUser.CompanyID;
						_data.SaveChanges();
						
						break;
                        
					}
				}
			}
            
            
            //var roleUser=_data.UserRoles.Where(x=>x.UserId==UserAndRoleListVM.applicationUser.Id).FirstOrDefault();
            //roleUser.RoleId = RoleID;
            //_data.SaveChanges();
			TempData["success"] = "Changed user role successfully ";
			return RedirectToAction(nameof(Index));
        }

	}
}
