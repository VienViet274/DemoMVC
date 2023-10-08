using DemoMVC.Data;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository
{
    public class CompanyRepository: Repository<Company>,ICompanyRepository 
    {
        private readonly DataContext _dataContext;
        public CompanyRepository(DataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }
        public  void Update(Company company)
        {
            _dataContext.Companies.Update(company);
        }

        public void Save()
        {
            _dataContext.SaveChanges();
        }
    }
}
