using Commons.BussinessClasses;
using System.Collections.Generic;

namespace Commons.Inferfaces.DAOs
{
    public interface IAverageVectorDao
    {
        IEnumerable<AverageVector> GetOverview();
        AverageVector GetDetail();
        void Add(AverageVector entity);
        void Delete(AverageVector entity);
    }
}
