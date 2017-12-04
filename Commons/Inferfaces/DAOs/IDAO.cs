using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Inferfaces.DAOs
{
    public interface IDAO<T>
    {
        IEnumerable<T> GetOverview();
        T GetDetail();
        void Add(T entity);
        void Delete(T entity);
    }
}
