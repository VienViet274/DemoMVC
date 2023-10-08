using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        T Get(Expression<Func<T, bool>> filter, string stringProperties,bool tracked=false);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string stringProperties);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
