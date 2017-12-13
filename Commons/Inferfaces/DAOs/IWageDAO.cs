using Commons.BussinessClasses;
using System.Collections.Generic;

namespace Commons.Inferfaces.DAOs
{
    public interface IWageDao
    {
        IEnumerable<Wage> GetOverview();
        Wage GetDetail();
        void Add(Wage entity);
        void Delete(Wage entity);
    }
}
