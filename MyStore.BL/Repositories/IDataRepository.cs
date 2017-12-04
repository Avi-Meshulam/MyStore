using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.BL.Repositories
{
    public interface IDataRepository<T>
    {
        List<T> GetAll();
        List<T> GetByPredicate(Func<T, bool> predicate);
        T GetById(uint id);
        T Add(T obj);
        bool Update(T obj);
        bool Delete(T obj);
        int Clear();
    }
}
