using Server.BussinessClasses;
using Server.Repositories;
using System;
using System.Collections.Generic;
using Server.DAO.Interfaces;

namespace Server.DAO
{
    public class EigenFaceDAO : IDAO<EigenFace>
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