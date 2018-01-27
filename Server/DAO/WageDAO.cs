using Commons.BussinessClasses;
using Commons.Inferfaces.DAOs;
using Server.Repositories;
using System;
using System.Collections.Generic;

namespace Server.DAO
{
    public class WageDao : IWageDao
    {
        GenericUnitOfWork _guow = new GenericUnitOfWork();
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

        public void DeleteAll()
        {
            _guow.Repository<Models.Wage>().DeleteAll();
            _guow.SaveChanges();
        }

        public IEnumerable<Wage> GetOverview()
        {
            var result = new List<Wage>();
            foreach (var wageModel in _guow.Repository<Models.Wage>().GetOverview())
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