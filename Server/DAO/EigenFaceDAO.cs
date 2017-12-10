using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System;
using System.Collections.Generic;

namespace Server.DAO
{
    public class EigenFaceDAO : IEigenFaceDAO
    {
        GenericUnitOfWork guow = new GenericUnitOfWork(); // zastosuj SipleInjector

        public void Add(EigenFace eigenFace)
        {
            guow.Repository<Models.EigenFace>().Add(
                        new Models.EigenFace
                        {
                            ID = eigenFace.Id,
                            Value = eigenFace.Value
                        }
                );

            guow.SaveChanges();
        }

        public void Delete(EigenFace eigenFace) ///Upewnic sie czy to napewno usuwa !!!!!
        {
            guow.Repository<Models.EigenFace>().Delete(
                        new Models.EigenFace
                        {
                            ID = eigenFace.Id,
                            Value = eigenFace.Value
                        }
                );

            guow.SaveChanges();
        }

        public EigenFace GetDetail()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EigenFace> GetOverview()
        {
            List<EigenFace> result = new List<EigenFace>();
            foreach(Models.EigenFace eigenFaceModel in guow.Repository<Models.EigenFace>().GetOverview())
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