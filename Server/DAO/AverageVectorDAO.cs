using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System.Collections.Generic;

namespace Server.DAO
{
    public class AverageVectorDao : IAverageVectorDao
    {
        GenericUnitOfWork _guow = new GenericUnitOfWork();

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

        public void DeleteAll()
        {
            _guow.Repository<Models.AverageVector>().DeleteAll();
            _guow.SaveChanges();
        }

        public IEnumerable<AverageVector> GetOverview()
        {
            var result = new List<AverageVector>();
            foreach (var averageVectorModel in _guow.Repository<Models.AverageVector>().GetOverview())
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