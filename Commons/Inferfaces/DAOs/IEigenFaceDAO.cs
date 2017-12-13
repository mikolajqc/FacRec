using Commons.BussinessClasses;
using System.Collections.Generic;

namespace Commons.Inferfaces.DAOs
{
    public interface IEigenFaceDao
    {
        IEnumerable<EigenFace> GetOverview();
        EigenFace GetDetail();
        void Add(EigenFace entity);
        void Delete(EigenFace entity);
    }
}
