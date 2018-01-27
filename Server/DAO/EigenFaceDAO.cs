using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System;
using System.Collections.Generic;

namespace Server.DAO
{
    public class EigenFaceDao : IEigenFaceDao
    {
        GenericUnitOfWork _guow = new GenericUnitOfWork();

        public void Add(EigenFace eigenFace)
        {
            _guow.Repository<Models.EigenFace>().Add(
                        new Models.EigenFace
                        {
                            ID = eigenFace.Id,
                            Value = eigenFace.Value
                        }
                );

            _guow.SaveChanges();
        }

        public void DeleteAll()
        {
            _guow.Repository<Models.EigenFace>().DeleteAll();
            _guow.SaveChanges();
        }

        public IEnumerable<EigenFace> GetOverview()
        {
            var result = new List<EigenFace>();
            foreach(var eigenFaceModel in _guow.Repository<Models.EigenFace>().GetOverview())
            {
                result.Add(
                    new EigenFace
                    {
                        Id = eigenFaceModel.ID,
                        Value = eigenFaceModel.Value
                    }
                );
            }
            return result;
        }
    }
}