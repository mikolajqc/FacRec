using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DAO.Interfaces
{
    public interface IDAO<T>
    {
        IEnumerable<T> GetOverview();
        T GetDetail();
        void Add(T eigenFace);
        void Delete(T eigenFace);
    }
}
