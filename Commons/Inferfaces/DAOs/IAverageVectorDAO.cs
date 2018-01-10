using Commons.BussinessClasses;
using System.Collections.Generic;

namespace Commons.Inferfaces.DAOs
{
    public interface IAverageVectorDao
    {
        IEnumerable<AverageVector> GetOverview();
        void Add(AverageVector entity);
        void DeleteAll();
    }
}
