using Commons.BussinessClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Inferfaces.DAOs
{
    public interface IWageDAO
    {
        IEnumerable<Wage> GetOverview();
        Wage GetDetail();
        void Add(Wage entity);
        void Delete(Wage entity);
    }
}
