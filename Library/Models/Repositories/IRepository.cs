using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public interface IRepository<T>
        where T: Identifiable
    {
        int Create(T entity);
        void Delete(int id);
        T GetByID(int id);
        IEnumerable<T> GetAll();
        int Save();
        void MarkModified(T entity);
        void Dispose();
    }
}
