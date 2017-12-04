using Commons.BussinessClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Inferfaces.DAOs
{
    public interface IEigenFaceDAO
    {
        IEnumerable<EigenFace> GetOverview();
        EigenFace GetDetail();
        void Add(EigenFace entity);
        void Delete(EigenFace entity);
    }
}
