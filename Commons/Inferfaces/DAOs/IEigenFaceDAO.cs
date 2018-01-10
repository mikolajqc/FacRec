using Commons.BussinessClasses;
using System.Collections.Generic;

namespace Commons.Inferfaces.DAOs
{
    public interface IEigenFaceDao
    {
        IEnumerable<EigenFace> GetOverview();
        void Add(EigenFace entity);
        void DeleteAll();
    }
}
