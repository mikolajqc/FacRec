using Commons.BussinessClasses;
using System.Collections.Generic;

namespace Commons.Inferfaces.DAOs
{
    public interface IWageDao
    {
        IEnumerable<Wage> GetOverview();
        void Add(Wage entity);
        void DeleteAll();
    }
}
