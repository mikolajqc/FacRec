using Server.BussinessClasses;
using Server.DAO.Interfaces;
using Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DAO
{
    public class WageDTO : IDAO<Wage>
    {
        GenericUnitOfWork guow = new GenericUnitOfWork(); // zastosuj SipleInjector chyba musi byc singleton???? poczytaj jak to powinno wygladac
        public void Add(Wage wage)
        {
            guow.Repository<Models.Wage>().Add(
                        new Models.Wage
                        {
                            ID = wage.Id,
                            Value = wage.Value
                        }
                );
        }

        public void Delete(Wage wage)
        {
            guow.Repository<Models.Wage>().Delete(
            new Models.Wage
            {
                        ID = wage.Id,
                        Value = wage.Value
                    }
                );
        }

        public Wage GetDetail()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Wage> GetOverview()
        {
            List<Wage> result = new List<Wage>();
            foreach (Models.Wage wageModel in guow.Repository<Models.Wage>().GetOverview())
            {
                result.Add(
                    new Wage
                    {
                        Id = wageModel.ID,
                        Value = wageModel.Value
                    }
                );
            }
            return result;
        }
    }
}