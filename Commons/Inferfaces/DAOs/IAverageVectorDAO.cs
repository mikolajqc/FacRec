using Commons.BussinessClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Inferfaces.DAOs
{
    public interface IAverageVectorDAO
    {
        IEnumerable<AverageVector> GetOverview();
        AverageVector GetDetail();
        void Add(AverageVector entity);
        void Delete(AverageVector entity);
    }
}
