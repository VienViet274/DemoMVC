using DemoMVC.Data;
using DemoMVC.Utility;
using DemoMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        public DBInitializer(DataContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            
            

        }
        public void Initialize()
        {
            //migration if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();    
                }
            }
            catch(Exception ex)
            {

            }
            //create role if they are not created
            if(!_roleManager.RoleExistsAsync(SD.Role_User_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company)).GetAwaiter().GetResult();

                
                var result = _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@dotnetmastery.com",
                    Email = "admin@dotnetmastery.com",
                    EmailConfirmed= true,
                    Name = "Bhrugen Patel",
                    PhoneNumber = "1112223333",
                    StreetAddress = "test 123 Ave",
                    State = "IL",
                    PostalCode = "23422",
                    City = "Chicago"
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUser.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");
                _userManager.AddToRoleAsync(user, SD.Role_User_Admin).GetAwaiter().GetResult();
            }

            // if roles are not created, then we will create admin user as well
            return;
            
        }
        
    }
}
