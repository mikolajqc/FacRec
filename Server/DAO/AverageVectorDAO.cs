using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System;
using System.Collections.Generic;

namespace Server.DAO
{
    public class AverageVectorDao : IAverageVectorDao
    {
        GenericUnitOfWork _guow = new GenericUnitOfWork(); // zastosuj SipleInjector

        public void Add(AverageVector averageVector)
        {
            _guow.Repository<Models.AverageVector>().Add(
                    new Models.AverageVector
                    {
                        ID = averageVector.Id,
                        Value = averageVector.Value
                    }
                );

            _guow.SaveChanges();
        }

        public void Delete(AverageVector averageVector)
        {
            _guow.Repository<Models.AverageVector>().Delete(
            new Models.AverageVector
            {
                        ID = averageVector.Id,
                        Value = averageVector.Value
                    }
                );

            _guow.SaveChanges();
        }

        public AverageVector GetDetail()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AverageVector> GetOverview()
        {
            List<AverageVector> result = new List<AverageVector>();
            foreach (Models.AverageVector averageVectorModel in _guow.Repository<Models.AverageVector>().GetOverview())
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