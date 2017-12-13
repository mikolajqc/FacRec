using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System;
using System.Collections.Generic;

namespace Server.DAO
{
    public class WageDao : IWageDao
    {
        GenericUnitOfWork _guow = new GenericUnitOfWork(); // zastosuj SipleInjector chyba musi byc singleton???? poczytaj jak to powinno wygladac
        public void Add(Wage wage)
        {
            _guow.Repository<Models.Wage>().Add(
                        new Models.Wage
                        {
                            ID = wage.Id,
                            Name = wage.Name,
                            Value = wage.Value
                        }
                );

            _guow.SaveChanges();
        }

        public void Delete(Wage wage)
        {
            _guow.Repository<Models.Wage>().Delete(
            new Models.Wage
            {
                        ID = wage.Id,
                        Name = wage.Name,
                        Value = wage.Value
                    }
                );

            _guow.SaveChanges();
        }

        public Wage GetDetail()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Wage> GetOverview()
        {
            List<Wage> result = new List<Wage>();
            foreach (Models.Wage wageModel in _guow.Repository<Models.Wage>().GetOverview())
            {
                result.Add(
                    new Wage
                    {
                        Id = wageModel.ID,
                        Name = wageModel.Name,
                        Value = wageModel.Value
                    }
                );
            }
            return result;
        }
    }
}