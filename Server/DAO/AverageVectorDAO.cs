using Server.BussinessClasses;
using Server.DAO.Interfaces;
using Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DAO
{
    public class AverageVectorDAO : IDAO<AverageVector>
    {
        GenericUnitOfWork guow = new GenericUnitOfWork(); // zastosuj SipleInjector

        public void Add(AverageVector averageVector)
        {
            guow.Repository<Models.AverageVector>().Add(
                    new Models.AverageVector
                    {
                        ID = averageVector.Id,
                        Value = averageVector.Value
                    }
                );
        }

        public void Delete(AverageVector averageVector)
        {
            guow.Repository<Models.AverageVector>().Delete(
            new Models.AverageVector
            {
                        ID = averageVector.Id,
                        Value = averageVector.Value
                    }
                );
        }

        public AverageVector GetDetail()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AverageVector> GetOverview()
        {
            List<AverageVector> result = new List<AverageVector>();
            foreach (Models.AverageVector averageVectorModel in guow.Repository<Models.AverageVector>().GetOverview())
            {
                result.Add(
                    new AverageVector
                    {
                        Id = averageVectorModel.ID,
                        Value = averageVectorModel.Value
                    }
                );
            }
            return result;
        }
    }
}