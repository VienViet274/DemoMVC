using DemoMVC.Data;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository
{
    public class ApplicationUserRepository:Repository<ApplicationUser>,IApplicationUserRepository
    {
        private readonly DataContext _ApplicationUserRepository;
        public ApplicationUserRepository(DataContext ApplicationUserRepository):base(ApplicationUserRepository)
        {
            _ApplicationUserRepository = ApplicationUserRepository;
        }

		public void Save()
		{
			_ApplicationUserRepository.SaveChanges();
		}

		public void Update(ApplicationUser applicationUser)
		{
			_ApplicationUserRepository.Update(applicationUser);
		}
	}
}
