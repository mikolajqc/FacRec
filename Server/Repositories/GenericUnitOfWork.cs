using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Repositories
{
    public class GenericUnitOfWork : IDisposable
    {
        private FaceRecognitionDatabaseEntities _db = null;
        public GenericUnitOfWork()
        {
            _db = new FaceRecognitionDatabaseEntities();
        }

        private Dictionary<Type, object> Repositories = new Dictionary<Type, object>();
        public IRepository<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)) == true)
                return Repositories[typeof(T)] as IRepository<T>;

            IRepository<T> repo = new GenericRepository<T>(_db);
            Repositories.Add(typeof(T), repo);
            return repo;
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        private bool _disposed = false;
        public void Dispose()
        {
            if (!this._disposed)
            {
                    _db.Dispose();
            }
            this._disposed = true;
        }
    }
}