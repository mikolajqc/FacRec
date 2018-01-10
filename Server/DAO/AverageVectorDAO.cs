using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System;
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

        //todo: zrob tu rzucanie entity exception gdy baza niedostepna i ogarnij jak to wyslac do klienta zeby pokazal dobry komunikat, bo narazie zwraca 500 error.
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