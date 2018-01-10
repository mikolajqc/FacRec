using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Server.Repositories
{
        public interface IRepository<T> where T : class
        {
            IEnumerable<T> GetOverview(Expression<Func<T, bool>> predicate = null);
            void Add(T entity);
            void DeleteAll();
        }
}
